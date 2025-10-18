using MoMA_thias.web.Model;
using Microsoft.EntityFrameworkCore;

namespace MoMA_thias.web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Art> Arts => Set<Art>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<Game> Games => Set<Game>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .Property(g => g.CurrentRoundArtId)
            .IsRequired(false);
    }
}