using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MoMA_thias.web.Model;
using MoMA_thias.web.Service;

namespace MoMA_thias.web.Components.Pages;

public class ControlGameBase : ComponentBase, IAsyncDisposable
{
    protected enum ControlView { Menu, Leaderboard, ArtSelection }
    
    [Inject] protected IGameService GameService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    
    private HubConnection? hubConnection;
    
    protected Game? Game { get; set; }
    protected IEnumerable<RankedBid> Leaderboard { get; private set; }= [];
    protected string Code { get; set; } = string.Empty;
    protected string? Error { get; set; }
    
    protected bool GameLoaded => Game is not null;
    
    protected ControlView CurrentView { get; set; } = ControlView.Menu;
    
    protected Guid? SelectedArtId { get => Game.CurrentRoundArtId; set => Game.CurrentRoundArtId = value; }
    
    protected IEnumerable<Art> AvailableArts => Game?.Arts ?? [];

    protected async Task Load()
    {
        Error = null;
        if (string.IsNullOrWhiteSpace(Code))
        {
            Error = "Enter a game code.";
            return;
        }

        Game = await GameService.GetGameFromGameCodeAsync(Code);
        if (Game is null)
        {
            Error = "Game not found.";
            return;
        }

        await SetupHubConnection();
    }

    protected void Clear()
    {
        Code = string.Empty;
        Error = null;
        Leaderboard = [];
    }
    
    protected void ResetToStart()
    {
        Game = null;
        Error = null;
        CurrentView = ControlView.Menu;
    }

    protected async void OnSelectArt(Guid artId)
    {
        if (Game is null) return;
        
        SelectedArtId = artId;
        await GameService.UpdateGameAsync(Game);
        await hubConnection.SendAsync("SelectArt", Game.GameCode);
        
        await InvokeAsync(StateHasChanged);   
    }
    
    protected async void RefreshLeaderboard()
    {
        if (Game is null) return;

        Leaderboard = await GameService.GetRankedPlayerBidsByGame(Game.Id);

        await InvokeAsync(StateHasChanged);   
    }
    
    private async Task SetupHubConnection()
    {
        hubConnection= new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/controlhub"))
            .Build();
        
        await hubConnection.StartAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}