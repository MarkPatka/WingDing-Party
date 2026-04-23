using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using EventService.Application.EventSourcing.IntegrationEventHandlers;
using EventService.Application.EventSourcing.IntegrationEvents.Incoming;
using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;
using EventService.Infrastructure.EventSourcing.Messaging;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddServices()
            .RegisterDbContext()
            .RegisterRepositories()
            .AddMessaging(configuration)
            ;
        return services;
    }


    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IConfigurationService, ConfigurationService>();
        
        services.AddScoped<IEventService, Services.EventService>();

        services.AddScoped<IEventTypeService, EventTypeService>();

        return services;
    }

    private static IServiceCollection AddMessaging(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection(KafkaOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IEventProducer, KafkaEventProducer>();
        services.AddSingleton<IDeadLetterQueueProducer, DeadLetterQueueProducer>();

        services.AddSingleton<IIntegrationEventTypeRegistry, IntegrationEventTypeRegistry>();
        services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
        services.AddSingleton<IEventConsumer, KafkaEventConsumer>();

        // Integration event handlers
        services.AddScoped<IIntegrationEventHandler<UserProfileUpdatedIntegrationEvent>,
            UserProfileUpdatedIntegrationEventHandler>();

        services.AddHostedService<KafkaEventConsumerBackgroundService>();

        return services;
    }

    private static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {

        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IRepository<Event, EventId>, 
                GenericRepository<Event, EventId>>()
            .AddScoped<IRepository<EventType, EventTypeId>, 
                GenericRepository<EventType, EventTypeId>>()
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

    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EventServiceDbContext>();
        context.Database.Migrate();
        return app;
    }
}
