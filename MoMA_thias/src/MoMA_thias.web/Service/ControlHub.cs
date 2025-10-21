using Microsoft.AspNetCore.SignalR;

namespace MoMA_thias.web.Service;

public class ControlHub : Hub
{
    public async Task JoinGame(string gameCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
    }

    public async Task SelectArt(string gameCode)
    {
        await Clients.Group(gameCode).SendAsync("SwitchArt");
    }
}