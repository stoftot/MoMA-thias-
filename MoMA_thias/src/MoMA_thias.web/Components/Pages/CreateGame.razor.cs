using Microsoft.AspNetCore.Components;
using MoMA_thias.web.Service;

namespace MoMA_thias.web.Components.Pages;

public class CreateGameBase : ComponentBase
{
    [Inject] protected IGameService GameService { get; set; } = default!;
    
    [Inject] private IArtService ArtService { get; set; } = default!;

    protected decimal[] Prices { get; } = new decimal[10];
    protected int Index { get; set; } = 0;
    protected bool Created { get; set; } = false;
    protected string? ValidationError { get; set; }
    
    private Guid GameId { get; set; } = Guid.Empty;
    protected string GameCode { get; set; } = string.Empty;
    protected string CurrentValueString
    {
        get => Prices[Index] == 0 ? string.Empty : Prices[Index].ToString("0.##");
        set
        {
            ValidationError = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                Prices[Index] = 0;
                return;
            }
            if (decimal.TryParse(value, out var parsed) && parsed >= 0)
            {
                Prices[Index] = parsed;
            }
            else
            {
                ValidationError = "Please enter a valid non-negative number.";
            }
        }
    }

    protected int CurrentIndexDisplay => Index + 1;
    protected bool IsFirst => Index == 0;
    protected bool IsLast => Index == 9;

    protected override async Task OnInitializedAsync()
    {
        GameCode = GameService.GenerateGameCode();
        GameId = (await GameService.CreateGameAsync(string.Empty, string.Empty, GameCode)).Id;
    }

    protected void Prev()
    {
        if (Index > 0) Index--;
    }

    protected void Next()
    {
        if (Prices[Index] <= 0)
        {
            ValidationError = "Price must be greater than 0.";
            return;
        }
        if (Index < 9) Index++;
    }

    protected async Task Finish()
    {
        if (Prices[Index] <= 0)
        {
            ValidationError = "Price must be greater than 0.";
            return;
        }

        for(int i = 0; i < Prices.Length; i++)
        {
            await ArtService.CreateArtAsync(GameId, $"Art Piece {i + 1}", (double)Prices[i]);
        }
        
        Created = true;
        StateHasChanged();
    }
}