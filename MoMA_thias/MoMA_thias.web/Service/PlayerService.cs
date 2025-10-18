using Microsoft.EntityFrameworkCore;
using MoMA_thias.web.Data;
using MoMA_thias.web.Model;

namespace MoMA_thias.web.Service;

interface IPlayerService
{
    Task<Player> CreatePlayerAsync(string name, string password);
    Task<Player> DeletePlayerAsync(Guid id);
    Task<Player> GetPlayerAsync(Guid id);
    Task<List<Player>> GetAllPlayers();

}

public class PlayerService : IPlayerService
{
    private readonly AppDbContext _db;
    public PlayerService(AppDbContext db) => _db = db;

    public async Task<Player> CreatePlayerAsync(string name, string password)
    {
        var p = new Player {
            Id = Guid.NewGuid(),
            Name = name,
            Password = password
        };
        _db.Players.Add(p);
        await _db.SaveChangesAsync();
        return p;
    }

    public async Task<Player> DeletePlayerAsync(Guid id)
    {
        var p = await _db.Players.FindAsync(id) ?? throw new KeyNotFoundException("Player not found");
        _db.Players.Remove(p);
        await _db.SaveChangesAsync();
        return p;
    }

    public async Task<List<Player>> GetAllPlayers()
    {
        return await _db.Players.ToListAsync();
    }

    public async Task<Player> GetPlayerAsync(Guid id)
    {
        var player = await _db.Players.FirstOrDefaultAsync(p => p.Id == id) ?? throw new KeyNotFoundException("Player not found");
        return player;
    }
}