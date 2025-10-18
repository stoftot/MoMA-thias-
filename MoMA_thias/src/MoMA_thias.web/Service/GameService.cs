using Microsoft.EntityFrameworkCore;
using MoMA_thias.web.Data;
using MoMA_thias.web.Model;

namespace MoMA_thias.web.Service;


// Record to hold ranked bid information, no time for result models :)
public record RankedBid(string PlayerName, string ArtName, double BidAmount, double Difference);


public interface IGameService
{
    Task<Game> CreateGameAsync(string title, string adminPassword, string gameCode);
    
    Task ResetGameAsync(Guid gameId);

    Task<Bid> PlaceBidAsync(Guid gameId, Guid artId, Guid playerId, double amount);

    Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByGame(Guid gameId);

    Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByArt(Guid gameId, Guid artId);

}

public class GameService : IGameService
{
    private readonly AppDbContext _db;

    public GameService(AppDbContext db) => _db = db;

 
    public async Task ResetGameAsync(Guid gameId)
    {
        var bids = _db.Bids.Where(b => b.GameId == gameId);
        _db.Bids.RemoveRange(bids);
        await _db.SaveChangesAsync();
    }

    public async Task<Bid> PlaceBidAsync(Guid playerId, Guid gameId, Guid artId, double amount)
    {
        var bid = new Bid { PlayerId = playerId, GameId = gameId, ArtId = artId, Amount = amount };
        _db.Bids.Add(bid);
        await _db.SaveChangesAsync();
        return bid;
    }

    public async Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByGame(Guid gameId)
    {
        return await GetRankedPlayerBidsByArt(gameId, Guid.Empty);
    }

    public async Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByArt(Guid gameId, Guid artId)
    {
        // hent alt, vi skal bruge, og lav opslags-tabeller
        var bids = artId == Guid.Empty
            ? await _db.Bids.Where(b => b.GameId == gameId).ToListAsync()
            : await _db.Bids.Where(b => b.GameId == gameId && b.ArtId == artId).ToListAsync();

        var playerIds = bids.Select(b => b.PlayerId).Distinct().ToList();
        var artIds = bids.Select(b => b.ArtId).Distinct().ToList();

        var players = await _db.Players
            .Where(p => playerIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Name);

        var arts = await _db.Arts
            .Where(a => artIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => new { a.Name, a.Price });

        return bids
            .Select(b => new RankedBid(
                players[b.PlayerId],
                arts[b.ArtId].Name,
                b.Amount,
                Math.Abs(b.Amount - arts[b.ArtId].Price)))
            .OrderBy(x => x.Difference)
            .ToList();
    }

    public Task<Game> CreateGameAsync(string title, string adminPassword, string gameCode)
    {
        var game = new Game { Id = Guid.NewGuid(), Title = title, AdminPassword = adminPassword, GameCode = gameCode };
        _db.Games.Add(game);
        _db.SaveChangesAsync();
        return Task.FromResult(game);
    }
}

