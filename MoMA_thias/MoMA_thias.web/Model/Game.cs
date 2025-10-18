namespace MoMA_thias.web.Model;

public class Game
{
    public Guid Id { get; set; }
 
    public required string Title { get; set; }

    public required string AdminPassword { get; set; }

    public required string GameCode { get; set; }

    public Guid CurrentRoundArtId { get; set; }

    public List<Art> Arts { get; set; } = [];
}