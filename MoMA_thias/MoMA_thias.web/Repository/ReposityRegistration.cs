using Microsoft.EntityFrameworkCore;

namespace MoMA_thias.web.Repository
{
    public static class ReposityRegistration
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {

            var home = Environment.GetEnvironmentVariable("HOME"); // Azure sætter HOME
            string dataDir = home is not null
                ? Path.Combine(home, "site", "data")
                : Path.Combine(AppContext.BaseDirectory, "App_Data");

            Directory.CreateDirectory(dataDir);
            var dbPath = Path.Combine(dataDir, "MomaThias.db");

            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlite($"Data Source={dbPath}"));


        }
    }
}