using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddServices()
            .RegisterRepositories()
            ;
        return services;
    }


    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddSingleton<ITimeProviderService, TimeProviderService>();

        services
            .AddTransient<IConfigurationService, ConfigurationService>();


        return services;
    }

    private static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {

        services
            .AddScoped<IEventRepository, EventRepository>();

        return services;
    }

}
