namespace MoMA_thias.web.Model;

public class Game
{
    public required Guid Id { get; set; }
 
    public required string Title { get; set; }

    public required string AdminPassword { get; set; }

    public required string GameCode { get; set; }

    // public Guid CurrentRoundArtId { get; set; }
    public Art? CurrentRoundArt { get; set; }
    
    public List<Art> Arts { get; set; } = [];
}