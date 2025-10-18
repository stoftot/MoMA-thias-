


// Just a record to hold ranked bid information, mo time for result models :)
public record RankedBid(string PlayerName, string ArtName, double BidAmount, double Difference);



public interface IGameService
{
    Task ResetGame(Guid gameId);

    Task<Bid> PlaceBidAsync(Guid gameId, Guid artId, Guid playerId, double amount);

    Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByGame(Guid gameId);

    Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByArt(Guid gameId, Guid artId);

}

public class GameService : IGameService
{
    public Task ResetGame(Guid gameId)
    {
        return Task.CompletedTask;
    }

    public Task<Bid> PlaceBidAsync(Guid gameId, Guid artId, Guid playerId, double amount)
    {
        return Task.FromResult<Bid>(default!);
    }

    public Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByGame(Guid gameId)
    {
        return Task.FromResult<IEnumerable<RankedBid>>(Array.Empty<RankedBid>());
    }

    public Task<IEnumerable<RankedBid>> GetRankedPlayerBidsByArt(Guid gameId, Guid artId)
    {
        return Task.FromResult<IEnumerable<RankedBid>>(Array.Empty<RankedBid>());
    }
}
