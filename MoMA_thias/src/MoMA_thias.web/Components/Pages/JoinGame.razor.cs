using Microsoft.AspNetCore.Components;
using MoMA_thias.web.Model;
using MoMA_thias.web.Service;

namespace MoMA_thias.web.Components.Pages;

public class JoinGameBase : ComponentBase
{
    [Inject] private IGameService GameService { get; set; } = default!;
    
    [Inject] private IArtService ArtService { get; set; } = default!;
    
    private Game? Game { get; set; }
    protected Art? CurrentArt { get; set; }
    
    protected string Code { get; set; } = string.Empty;
    protected string DisplayName { get; set; } = string.Empty;
    protected string? StartError { get; set; }
    protected string? ValidationError { get; set; }

    private decimal Guess { get; set; } = 0;
    
    protected bool BidSubmitted => GameService.HavePlacedBidAsync(DisplayName, Game.Id, Game.CurrentRoundArtId.Value);

    protected bool Started { get; set; }
    
    protected string CurrentValueString
    {
        get => Guess == 0 ? string.Empty : Guess.ToString("0.##");
        set
        {
            ValidationError = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                Guess = 0;
                return;
            }
            if (decimal.TryParse(value, out var parsed) && parsed >= 0)
                Guess = parsed;
            else
                ValidationError = "Please enter a valid non-negative number.";
        }
    }

    protected async void Start()
    {
        StartError = null;

        if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(DisplayName))
        {
            StartError = "Please enter a game code and display name.";
            return;
        }
        
        Game = await GameService.GetGameFromGameCodeAsync(Code);
        if (Game is null)
        {
            StartError = "Game not found or not ready yet.";
            return;
        }
        
        if (await GameService.IsNameTakenAsync(DisplayName, Game.Id))
        {
            StartError = "Display name is already taken in this game.";
            
            return;
        }
        
        if (Game.CurrentRoundArtId is not null)
            CurrentArt = await ArtService.GetArtAsync(Game.CurrentRoundArtId.Value);
        
        Started = true;
        StateHasChanged();
    }

    protected async Task SubmitBid()
    {
        await GameService.PlaceBidAsync(DisplayName, Game.Id, Game.CurrentRoundArtId.Value, (double)Guess);
        Guess = 0;
        StateHasChanged();
    }

    protected async Task Refresh()
    {
        Game = await GameService.GetGameFromIdAsync(Game.Id);
        CurrentArt = await ArtService.GetArtAsync(Game.CurrentRoundArtId.Value);
        StateHasChanged();
    }
}