namespace MoMA_thias.web.Model;

public class Bid
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string PlayerName { get; set; }
    public Guid ArtId { get; set; }
    public double Amount { get; set; }
}