using EventService.Application.Common.Configuration;
using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddServices()
            .RegisterDbContext()
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
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IRepository<Event, EventId>, GenericRepository<Event, EventId>>()
            ;

        return services;
    }

    private static IServiceCollection RegisterDbContext(this IServiceCollection services)
    {
        services.AddDbContextFactory<EventServiceDbContext>((provider, options) =>
        {
            var dbSettings = provider
                .GetRequiredService<IOptions<EventsDatabaseConnection>>().Value;

            options.UseNpgsql(dbSettings.CONNECTION_STRING, cfg => cfg.EnableRetryOnFailure(2));

        }, ServiceLifetime.Scoped);

        return services;
    }

}
