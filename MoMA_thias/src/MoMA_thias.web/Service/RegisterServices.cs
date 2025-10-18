namespace MoMA_thias.web.Service;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {
      services.AddScoped<IGameService, GameService>();
      services.AddScoped<IArtService, ArtService>();
      services.AddScoped<IPlayerService, PlayerService>();
    }
}