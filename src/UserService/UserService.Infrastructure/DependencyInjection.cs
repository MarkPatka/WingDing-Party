using ClubService.Infrastructure.Messaging.Mapping;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using UserService.Application.Common.Configuration;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Persistence.Outbox;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Storage;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddMappings();
        services.RegisterRepositories();
        services.RegisterStorages(configuration);
        services.RegisterDbContext();
        services.BackgroundServices();
        services.MessagingServices(configuration);
        return services;
    }


    private static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<UserProfile, UserId>, GenericRepository<UserProfile, UserId>>();
        return services;
    }

    private static IServiceCollection RegisterStorages(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMinioBucketManager, MinioBucketManager>();
        services.AddSingleton<IFileStorage, MinioFileStorage>();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
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
            .Bind(configuration.GetSection(nameof(KafkaOptions)))
            .ValidateOnStart();
        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = new TypeAdapterConfig();
        config.Scan(typeof(UserIntegrationMappingConfiguration).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }

    private static IServiceCollection RegisterDbContext(this IServiceCollection services)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContextFactory<UserServiceDbContext>((provider, options) =>
        {
            var dbSettings = provider
                .GetRequiredService<IOptions<EventsDatabaseOptions>>().Value;

            options.UseNpgsql(dbSettings.DB_CONNECTION_STRING, cfg => cfg.EnableRetryOnFailure(2));
        }, ServiceLifetime.Scoped);

        return services;
    }

    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserServiceDbContext>();
        context.Database.Migrate();
        return app;
    }
}