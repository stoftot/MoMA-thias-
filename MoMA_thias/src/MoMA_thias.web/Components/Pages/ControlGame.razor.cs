using Microsoft.AspNetCore.Components;
using MoMA_thias.web.Model;
using MoMA_thias.web.Service;

namespace MoMA_thias.web.Components.Pages;

public class ControlGameBase : ComponentBase
{
    protected enum ControlView { Menu, Leaderboard, ArtSelection }
    
    [Inject] protected IGameService GameService { get; set; } = default!;
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
        Leaderboard = await GameService.GetRankedPlayerBidsByGame(Game.Id);
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

    protected void EnsureArtLoaded()
    {
        if (Game is null) return;
    }

    protected async void OnSelectArt(Guid artId)
    {
        if (Game is null) return;
        
        SelectedArtId = artId;
        await GameService.UpdateGameAsync(Game);
        
        StateHasChanged();
    }
    
    protected async void RefreshLeaderboard()
    {
        if (Game is null) return;

        // Re-fetch the game from the GameService by code to update guesses
        var updated = await GameService.GetGameFromIdAsync(Game.Id);
        if (updated is not null)
            Game = updated;

        StateHasChanged();
    }

    // Switchers invoked from the buttons
    protected void GoLeaderboard()
    {
        CurrentView = ControlView.Leaderboard;
    }

    protected void GoArtSelection()
    {
        CurrentView = ControlView.ArtSelection;
        EnsureArtLoaded();
    }
}