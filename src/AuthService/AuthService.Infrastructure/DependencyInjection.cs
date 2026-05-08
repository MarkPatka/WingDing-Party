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
            .AddHttpClients()
            .AddAuthorization();
        
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

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
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

    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        context.Database.Migrate();
        return app;
    }
}
