using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using MoMA_thias.web.Data;
using MoMA_thias.web.Model;

namespace MoMA_thias.web.Service;



public sealed record RankedBid (
    int Rank,
    string PlayerName,
    decimal TotalDifference,
    int BidCount
);


public interface IGameService
{
    Task<Game> CreateGameAsync(string title, string adminPassword);

    Task ResetGameAsync(Guid gameId);

    Task<Bid> PlaceBidAsync(string playerName, Guid gameId, Guid artId, decimal amount);

    bool HavePlacedBidAsync(string playerName, Guid gameId, Guid artId);

    Task<bool> IsNameTakenAsync(string playerName, Guid gameId);

    Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByGame(Guid gameId);

    Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByArt(Guid gameId, Guid artId);

    Task<Game?> GetGameFromGameCodeAsync(string gameCode);
    Task<Game?> GetGameFromIdAsync(Guid gameId);

    Task UpdateGameAsync(Game game);
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

    public async Task<Bid> PlaceBidAsync(string playerName, Guid gameId, Guid artId, decimal amount)
    {
        var bid = new Bid { PlayerName = playerName, GameId = gameId, ArtId = artId, Amount = amount };
        _db.Bids.Add(bid);
        await _db.SaveChangesAsync();
        return bid;
    }
    
    public bool HavePlacedBidAsync(string playerName, Guid gameId, Guid artId)
    {
        return _db.Bids.Any(b => b.PlayerName == playerName && b.GameId == gameId && b.ArtId == artId);
    }

    public Task<bool> IsNameTakenAsync(string playerName, Guid gameId)
    {
        return _db.Bids.AnyAsync(b => b.PlayerName == playerName && b.GameId == gameId); 
    }

    public async Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByGame(Guid gameId)
    {
        return await GetRankedPlayerBidsByArt(gameId, Guid.Empty);
    }

    public async Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByArt(Guid gameId, Guid artId)
    {

        // Join and compute |Amount - Price| without Math.Abs (SQLite-friendly)
        var baseQuery =
            from b in _db.Bids.AsNoTracking()
            join a in _db.Arts.AsNoTracking() on b.ArtId equals a.Id
            where b.GameId == gameId //&& (artId == Guid.Empty || b.ArtId == artId)
            let diff = (b.Amount >= a.Price) ? (b.Amount - a.Price) : (a.Price - b.Amount) // <-- no Math.Abs
            select new
            {
                b.PlayerName,
                Diff = diff
            };
        


        var bids = await baseQuery.ToListAsync();

        // 3. Aggreger per spiller
        var aggregated = bids
            .GroupBy(x => x.PlayerName)
            .Select(g => new
            {
                PlayerName = g.Key,
                TotalDifference = g.Sum(v => v.Diff),
                BidCount = g.Count()
            })
            .OrderBy(x => x.TotalDifference)
            .ThenBy(x => x.PlayerName)
            .ToList();

        // 4. Tilføj rank (dense rank: 1,1,2,3…)
        var ranked = new List<RankedBid>(aggregated.Count);
        decimal? prev = null;
        int rank = 0;
        foreach (var x in aggregated)
        {
            if (prev is null || x.TotalDifference != prev.Value)
            {
                rank = ranked.Count == 0 ? 1 : rank + 1;
                prev = x.TotalDifference;
            }
            ranked.Add(new RankedBid(rank, x.PlayerName, x.TotalDifference, x.BidCount));
        }

    return ranked;
    }
    

    public Task<Game> CreateGameAsync(string title, string adminPassword)
    {
        var game = new Game { Id = Guid.NewGuid(), Title = title, AdminPassword = adminPassword, GameCode = GenerateGameCode() };
        _db.Games.Add(game);
        _db.SaveChangesAsync();
        return Task.FromResult(game);
    }
    
    public async Task<Game?> GetGameFromGameCodeAsync(string gameCode)
    {
        var game = await _db.Games
            .AsNoTracking()
            .Include(g => g.Arts)
            .FirstOrDefaultAsync(g => g.GameCode == gameCode);   
        return game; 
    }
    
    public async Task<Game?> GetGameFromIdAsync(Guid gameId)
    {
        var game = await _db.Games
            .AsNoTracking()
            .Include(g => g.Arts)
            .FirstOrDefaultAsync(g => g.Id == gameId);   
        return game;
    }

    public async Task UpdateGameAsync(Game gameModel)
    {
        var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == gameModel.Id);
        if (game is null) return;

        game.CurrentRoundArtId = gameModel.CurrentRoundArtId;
        await _db.SaveChangesAsync();    
    }

    private string GenerateGameCode()
    {
        const string chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
        Span<char> code = stackalloc char[6];

        long ticks = DateTime.UtcNow.Ticks;
        byte[] hash = SHA256.HashData(BitConverter.GetBytes(ticks));

        for (int i = 0; i < 6; i++)
        {
            code[i] = chars[hash[i] % chars.Length];
        }
        
        return new string(code);
    }
}

