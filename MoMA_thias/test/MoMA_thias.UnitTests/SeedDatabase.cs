using System;
using MoMA_thias.web.Data;
using MoMA_thias.web.Model;

namespace MoMA_thias.UnitTests;

public class DatabaseSetup
{
    public static async Task<Game> CreateGame(AppDbContext db)
    {
        var game = new Game { Id = Guid.NewGuid(), Title = "Spil 1", AdminPassword = "pw", GameCode = "ABC" };

        db.Games.Add(game);

        db.Players.AddRange(
            new Player { Id = Guid.NewGuid(), Name = "Alice", Password = "alicepw" },
            new Player { Id = Guid.NewGuid(), Name = "Bob", Password = "bobpw" }
        );
        
        db.Arts.AddRange(
            new Art { GameId = game.Id,  Id = Guid.NewGuid(), Name = "Mona Lisa", Price = 100 },
            new Art { GameId = game.Id, Id = Guid.NewGuid(), Name = "Starry Night", Price = 200 }
        );

        await db.SaveChangesAsync();

        return game;
    }
}
 