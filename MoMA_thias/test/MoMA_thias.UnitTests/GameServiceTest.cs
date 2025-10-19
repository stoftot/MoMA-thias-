using Microsoft.EntityFrameworkCore;
using MoMA_thias.UnitTests.TestUtilities;
using MoMA_thias.web.Data;
using MoMA_thias.web.Service;

namespace MoMA_thias.UnitTests;

public class GameServiceTest : IDisposable
{
   
    private readonly AppDbContext _db;

    public GameServiceTest()
    {
        var (ctx, conn) = SqliteInMemoryContextFactory.CreateContext();
        _db = ctx;
    }

    [Fact]
    public async Task CreateGameAsync_opretter_game_med_id()
    {

        var _service = new GameService(_db);

        var game = await _service.CreateGameAsync("Auction 2", "AdminPassword", "AWERFG");

        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("Auction 2", game.Title);
        Assert.Equal("AWERFG", game.GameCode);
        Assert.Equal("AdminPassword", game.AdminPassword);

        Assert.Single(_db.Games);
    }

    [Fact]
    public async Task GetRankedPlayerBidsByGame_henter_bud_for_spil()
    {

        var _service = new GameService(_db);

        var game = await _service.CreateGameAsync("Spil 1", "AdminPassword", "AWERFG");

        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("Spil 1", game.Title);
        Assert.Equal("AWERFG", game.GameCode);
        Assert.Equal("AdminPassword", game.AdminPassword);

        Assert.Single(_db.Games);
    }
  
    [Fact]
    public async Task PlaceBidAsync_opretter_bid_og_returnerer_det()
    {
        var game = await DatabaseSetup.CreateGame(_db);

        var gameId = game.Id;

        var _service = new GameService(_db);

        var p1 = _db.Players.First().Id;
        var aMona = _db.Arts.First().Id;
        var bid = await _service.PlaceBidAsync(p1, gameId, aMona, 123.45);

        Assert.Equal(123.45, bid.Amount);
        Assert.Equal(p1, bid.PlayerId);
        Assert.Equal(aMona, bid.ArtId);
        Assert.Equal(gameId, bid.GameId);}    

    [Fact]
    public async Task GetRankedPlayerBidsByGame()
    {
        var game = await DatabaseSetup.CreateGame(_db);

        var gameId = game.Id;

        var _service = new GameService(_db);

        var p1 = _db.Players.Single(p => p.Name.ToLower().Equals("alice")).Id;
        var aMona = _db.Arts.Single(a => a.Name.ToLower().Equals("mona lisa")).Id;
        var bid = await _service.PlaceBidAsync(p1, gameId, aMona, 50.0);

        var p2 = _db.Players.Single(p => p.Name.ToLower().Equals("bob")).Id;
        var starry = _db.Arts.Single(a => a.Name.ToLower().Equals("starry night")).Id;
        bid = await _service.PlaceBidAsync(p2, gameId, starry, 100.0);


        var bids = await _service.GetRankedPlayerBidsByGame(gameId);

        Assert.NotNull(bids);
        Assert.Equal(2, bids.Count());
        Assert.Equal("Alice", bids.First().PlayerName);
        Assert.Equal("Mona Lisa", bids.First().ArtName);
        Assert.Equal(50, bids.First().BidAmount);  
    }

    public void Dispose()
    {
        _db.Dispose();
    }    
   
}
