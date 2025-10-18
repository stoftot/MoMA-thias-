namespace MoMA_thias.web.Model;

public class Art
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public required string Name { get; set; }
    public double Price { get; set; }
}