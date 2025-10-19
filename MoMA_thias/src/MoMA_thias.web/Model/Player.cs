namespace MoMA_thias.web.Model;

public class Player
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Password { get; set; }
}