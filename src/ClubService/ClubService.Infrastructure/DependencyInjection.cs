using System.Reflection;
using ClubService.Application.Common.Configuration;
using ClubService.Application.Persistence;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;
using ClubService.Infrastructure.Messaging;
using ClubService.Infrastructure.Messaging.Mapping;
using ClubService.Infrastructure.Persistence;
using ClubService.Infrastructure.Persistence.Outbox;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClubService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        //services.AddMappings();
        services.RegisterRepositories();
        services.RegisterDbContext();
        services.BackgroundServices();
        services.MessagingServices(configuration);
        return services;
    }


    private static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Club, ClubId>, GenericRepository<Club, ClubId>>();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IClubService, ClubService.Infrastructure.Services.ClubService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = new TypeAdapterConfig();
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }

    private static IServiceCollection BackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessorBackgroundService>();
        return services;
    }

    private static IServiceCollection MessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventTypeMapper, EventTypeMapper>();
        services.AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
        services.AddSingleton<IValidateOptions<Dictionary<string, KafkaOptions>>, KafkaOptionsValidator>();
        services.AddScoped<IIntegrationEventDispatcher, KafkaIntegrationEventDispatcher>();

        services.AddOptions<Dictionary<string, KafkaOptions>>()
            .Bind(configuration.GetSection("KafkaOptions"))
            .ValidateOnStart();
        return services;
    }

    private static IServiceCollection RegisterDbContext(this IServiceCollection services)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContextFactory<UserServiceDbContext>((provider, options) =>
        {
            var dbSettings = provider
                .GetRequiredService<IOptions<EventsDatabaseOptions>>().Value;
            options.UseNpgsql(dbSettings.CONNECTION_STRING, cfg => cfg.EnableRetryOnFailure(2));
        }, ServiceLifetime.Scoped);

        return services;
    }
}