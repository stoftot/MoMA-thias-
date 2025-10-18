interface IPlayerService
{
    Task<Player> CreatePlayerAsync(string gameCode, string name, string password);
    Task<Player> DeletePlayerAsync(Guid id);

}