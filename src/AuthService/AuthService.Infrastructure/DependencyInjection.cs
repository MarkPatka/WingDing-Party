using AuthService.Application.Common.Interfaces;
using AuthService.Application.Persistence;
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
            .BindConfigurations(configuration)
            .RegisterRedis()
            .AddAuthentication()
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

    private static IServiceCollection BindConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        // from .env
        services.Configure<AuthDatabaseOptions>(configuration.Bind);
        services.Configure<RedisOptions>(configuration.Bind);

        // from json settings
        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.SectionName));
        services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.SectionName));

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

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

        services.AddScoped<IAuthDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<AuthDbContext>>().CreateDbContext());

        return services;
    }

    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AuthDbContext>>();
        using AuthDbContext context = factory.CreateDbContext();
        context.Database.Migrate();
        return app;
    }
}
