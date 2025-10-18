using Microsoft.EntityFrameworkCore;
using MoMA_thias.web.Data;
using MoMA_thias.web.Model;

namespace MoMA_thias.web.Service;

public interface IArtService
{
    Task<Art> CreateArtAsync(Guid gameId, string name, double price);
    Task<Art> UpdateArtAsync(Guid id, string name, double price);
    Task<Art> DeleteArtAsync(Guid id);
    Task<Art> GetArtAsync(Guid id);
}

public class ArtService : IArtService
{
    private readonly AppDbContext _db;
    public ArtService(AppDbContext db) => _db = db;

    public async Task<Art> CreateArtAsync(Guid gameId, string name, double price)
    {
        var a = new Art { GameId = gameId, Name = name, Price = price };
        _db.Arts.Add(a);
        await _db.SaveChangesAsync();
        return a;
    }

    public async Task<Art> UpdateArtAsync(Guid artId, string name, double price)
    {
        var a = await _db.Arts.FirstAsync(x => x.Id == artId);
        a.Name = name; a.Price = price;
        await _db.SaveChangesAsync();
        return a;
    }

    public async Task<Art> DeleteArtAsync(Guid id)
    {
        var a = await _db.Arts.FindAsync(id) ?? throw new KeyNotFoundException("Art not found");
        _db.Arts.Remove(a);
        await _db.SaveChangesAsync();
        return a;
    }
    
    public async Task<Art> GetArtAsync(Guid id)
    {
        var a = await _db.Arts.FindAsync(id) ?? throw new KeyNotFoundException("Art not found");
        return a;
    }
}