using Microsoft.AspNetCore.Components;
using MoMA_thias.web.Model;
using MoMA_thias.web.Service;

namespace MoMA_thias.web.Components.Pages;

public class ControlGameBase : ComponentBase
{
    [Inject] protected IGameService GameService { get; set; } = default!;
    protected IEnumerable<RankedBid> Leaderboard { get; private set; }= [];
    protected string Code { get; set; } = string.Empty;
    protected string? Error { get; set; }

    protected async Task Load()
    {
        Error = null;
        if (string.IsNullOrWhiteSpace(Code))
        {
            Error = "Enter a game code.";
            return;
        }
        var gameId = await GameService.GetGameIdFromCodeAsync(Code);
        if (gameId == Guid.Empty)
        {
            Error = "Game not found.";
            return;
        }
        Leaderboard = await GameService.GetRankedPlayerBidsByGame(gameId);
    }

    protected void Clear()
    {
        Code = string.Empty;
        Error = null;
        Leaderboard = [];
    }
}