namespace MoMA_thias.web.Service;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {

      services.AddScoped<IGameService, GameService>();


    }
}