using AuthService.Application.Common.Interfaces;
using AuthService.Application.Services;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Authorization;
using AuthService.Infrastructure.Common.Configuration;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WingDing.Auth.Shared;
using WingDing.Auth.Shared.Services;
using AuthenticationOptions = AuthService.Infrastructure.Common.Configuration.AuthenticationOptions;
using AuthenticationService = AuthService.Infrastructure.Services.AuthenticationService;
using IAuthenticationService = AuthService.Application.Services.IAuthenticationService;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .RegisterDbContext()
            .RegisterRedis()
            .AddAuthentication(configuration)
            .AddAuthorization()
            .AddHttpClients();

        return services;
    }
    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();
        services.AddScoped<IPermissionService, LocalPermissionService>();
        services.AddWingDingAuthCore();
        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // TODO: move to .env or secret manager
        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.AddTransient<AdminAuthorizationDelegatingHandler>();
        services.AddScoped<IUserContext, UserContext>();
        return services;
    }

    private static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        // HttpClient for Keycloak Admin API (user registration)
        services.AddHttpClient<IAuthenticationService, AuthenticationService>(
            (sp, httpClient) =>
            {
                var opts = sp.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                httpClient.BaseAddress = new Uri(opts.AdminUrl);
            })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        // HttpClient for Keycloak Token endpoint (login)
        services.AddHttpClient<IJwtService, JwtService>(
            (sp, httpClient) =>
            {
                var opts = sp.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                httpClient.BaseAddress = new Uri(opts.TokenUrl);
            });

        services.AddHttpContextAccessor();

        return services;
    }

    private static IServiceCollection RegisterRedis(this IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            var redisOptions = services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<RedisOptions>>().Value;

            options.Configuration = redisOptions.REDIS_CONNECTION_STRING;
        });

        return services;
    }

    private static IServiceCollection RegisterDbContext(this IServiceCollection services)
    {
        services.AddDbContextFactory<AuthDbContext>((provider, options) =>
        {
            var dbSettings = provider
                .GetRequiredService<IOptions<AuthDatabaseOptions>>().Value;

            options.UseNpgsql(dbSettings.CONNECTION_STRING, cfg => cfg.EnableRetryOnFailure(2))
            .UseSnakeCaseNamingConvention();
        
        }, ServiceLifetime.Scoped);

        return services;
    }
/*
  Npgsql.PostgresException
  HResult=0x80004005
  Message=42703: column "migration_id" does not exist

POSITION: 8
  Source=Npgsql
  StackTrace:
   at Npgsql.Internal.NpgsqlConnector.<ReadMessageLong>d__234.MoveNext()
   at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
   at Npgsql.NpgsqlDataReader.<NextResult>d__52.MoveNext()
   at Npgsql.NpgsqlDataReader.<NextResult>d__52.MoveNext()
   at Npgsql.NpgsqlDataReader.NextResult()
   at Npgsql.NpgsqlCommand.<ExecuteReader>d__122.MoveNext()
   at Npgsql.NpgsqlCommand.<ExecuteReader>d__122.MoveNext()
   at System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult()
   at Npgsql.NpgsqlCommand.ExecuteReader(CommandBehavior behavior)
   at Npgsql.NpgsqlCommand.ExecuteDbDataReader(CommandBehavior behavior)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReader(RelationalCommandParameterObject parameterObject)
   at Microsoft.EntityFrameworkCore.Migrations.HistoryRepository.GetAppliedMigrations()
   at Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal.NpgsqlHistoryRepository.GetAppliedMigrations()
   at Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal.NpgsqlMigrator.Migrate(String targetMigration)
   at Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.Migrate(DatabaseFacade databaseFacade)
   at AuthService.Infrastructure.DependencyInjection.ApplyMigrations(IApplicationBuilder app) in D:\Repositories\WingDingRepository\WingDing-Party\src\AuthService\AuthService.Infrastructure\DependencyInjection.cs:line 120
   at Program.<Main>$(String[] args) in D:\Repositories\WingDingRepository\WingDing-Party\src\AuthService\AuthService.Api\Program.cs:line 27

*/



    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AuthDbContext>>();
        using AuthDbContext context = factory.CreateDbContext();
        context.Database.Migrate();
        return app;
    }
}
