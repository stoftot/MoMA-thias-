namespace MoMA_thias.web.Model;

public class Bid
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public required string PlayerName { get; set; }
    public Guid ArtId { get; set; }
    public decimal Amount { get; set; }
}