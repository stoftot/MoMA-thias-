using Microsoft.AspNetCore.Components;
using MoMA_thias.web.Model;
using MoMA_thias.web.Service;

namespace MoMA_thias.web.Components.Pages;

public class JoinGameBase : ComponentBase
{
    [Inject] private IGameService GameService { get; set; } = default!;
    
    private Game? Game { get; set; }
    
    protected string Code { get; set; } = string.Empty;
    protected string DisplayName { get; set; } = string.Empty;

    protected bool Started { get; set; }
    protected bool Finished { get; set; }
    protected string? StartError { get; set; }
    protected string? ValidationError { get; set; }

    protected decimal[] Guesses { get; } = new decimal[10];
    protected int Index { get; set; }

    protected string CurrentValueString
    {
        get => Guesses[Index] == 0 ? string.Empty : Guesses[Index].ToString("0.##");
        set
        {
            ValidationError = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                Guesses[Index] = 0;
                return;
            }
            if (decimal.TryParse(value, out var parsed) && parsed >= 0)
                Guesses[Index] = parsed;
            else
                ValidationError = "Please enter a valid non-negative number.";
        }
    }

    protected int CurrentIndexDisplay => Index + 1;
    protected bool IsFirst => Index == 0;
    protected bool IsLast => Index == 9;

    protected async void Start()
    {
        StartError = null;

        if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(DisplayName))
        {
            StartError = "Please enter a game code and display name.";
            return;
        }

        Game = await GameService.GetGameFromGameCode(Code);
        if (Game is null)
        {
            StartError = "Game not found or not ready yet.";
            return;
        }

        // In a more advanced build you might lock names, etc.
        Started = true;
    }

    protected void Prev()
    {
        if (Index > 0) Index--;
    }

    protected void Next()
    {
        if (Guesses[Index] <= 0)
        {
            ValidationError = "Guess must be greater than 0.";
            return;
        }
        if (Index < 9) Index++;
    }

    protected void Finish()
    {
        if (Guesses[Index] <= 0)
        {
            ValidationError = "Guess must be greater than 0.";
            return;
        }

        // Save guess
        Finished = true;
    }
}