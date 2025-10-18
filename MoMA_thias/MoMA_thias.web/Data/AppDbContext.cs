using MoMA_thias.web.Model;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Art> Arts => Set<Art>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<Game> Games => Set<Game>();
}