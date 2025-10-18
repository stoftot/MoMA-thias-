using MoMA_thias.web.Model;

namespace MoMA_thias.web.Service;

interface IPlayerService
{
    Task<Player> CreatePlayerAsync(string gameCode, string name, string password);
    Task<Player> DeletePlayerAsync(Guid id);

}