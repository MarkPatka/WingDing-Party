using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using UserService.Application.Common.Configuration;
using UserService.Application.IntegrationEvents.Mapping;
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
    private static int _repositoriesRegistered;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddIntegrationEventMappers();
        services.RegisterRepositories();
        services.RegisterFileStorages(configuration);
        services.RegisterDbContext();
        services.BackgroundServices();
        services.MessagingServices(configuration);

        return services;
    }


    private static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        if (Interlocked.Exchange(ref _repositoriesRegistered, 1) == 1) return services;

        services.AddScoped<IRepository<UserProfile, UserId>, GenericRepository<UserProfile, UserId>>();
        return services;
    }

    private static IServiceCollection RegisterFileStorages(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMinioBucketManager, MinioBucketManager>();
        services.AddSingleton<IFileStorage, MinioFileStorage>();
        services.AddSingleton<IMinioClient>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<FileStorageOptions>>().Value;
            return new MinioClient()
                .WithEndpoint(opt.Endpoint)
                .WithCredentials(opt.AccessKey, opt.SecretKey)
                .WithSSL(opt.WithSsl)
                .Build();
        });
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

    private static IServiceCollection AddIntegrationEventMappers(
        this IServiceCollection services)
    {
        // Регистрируем типорезолвер
        services.AddSingleton<IIntegrationEventTypeResolver, IntegrationEventTypeResolver>();

        // Mapster-конфигурация подхватится автоматически через
        // TypeAdapterConfig.Scan(assembly) или через явную регистрацию IRegister
        return services;
    }

    private static IServiceCollection RegisterDbContext(this IServiceCollection services)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        services.AddDbContext<UserServiceDbContext>((provider, options) =>
        {
            var dbSettings = provider
                .GetRequiredService<IOptions<UserDatabaseOptions>>().Value;

            options.UseNpgsql(dbSettings.CONNECTION_STRING, cfg => cfg.EnableRetryOnFailure(2));
        }, ServiceLifetime.Scoped);

        services.AddDbContextFactory<UserServiceDbContext>((provider, options) =>
        {
            var dbSettings = provider
                .GetRequiredService<IOptions<UserDatabaseOptions>>().Value;

            options.UseNpgsql(dbSettings.CONNECTION_STRING, cfg => cfg.EnableRetryOnFailure(2));

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