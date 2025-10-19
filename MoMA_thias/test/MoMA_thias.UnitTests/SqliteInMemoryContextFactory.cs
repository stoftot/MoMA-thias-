using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MoMA_thias.web.Data;

namespace MoMA_thias.UnitTests.TestUtilities
{
    public static class SqliteInMemoryContextFactory
    {
        public static (AppDbContext ctx, SqliteConnection conn) CreateContext()
        {
            // Opret en in-memory SQLite database (forsvinder når forbindelsen lukkes)
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .EnableSensitiveDataLogging()
                .LogTo(msg => Console.WriteLine(msg)) // eller Console.WriteLine
                .Options;

            var ctx = new AppDbContext(options);
            ctx.Database.EnsureCreated();

            return (ctx, connection);
        }
    }
}
