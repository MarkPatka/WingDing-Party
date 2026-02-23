using EventService.Application.Common.Configuration;
using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
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
            .AddTransient<IConfigurationService, ConfigurationService>();
        
        services
            .AddScoped<IEventService, Services.EventService>();

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
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<EventServiceDbContext>((provider, options) =>
        {
            var dbSettings = provider
                .GetRequiredService<IOptions<EventsDatabaseOptions>>().Value;

            options.UseNpgsql(dbSettings.CONNECTION_STRING, cfg => cfg.EnableRetryOnFailure(2));
        }, ServiceLifetime.Scoped);

        services.AddDbContextFactory<EventServiceDbContext>((provider, options) =>
        {
            var dbSettings = provider
                .GetRequiredService<IOptions<EventsDatabaseOptions>>().Value;

            options.UseNpgsql(dbSettings.CONNECTION_STRING, cfg => cfg.EnableRetryOnFailure(2));

        }, ServiceLifetime.Scoped);

        return services;
    }

}
