using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WingDing.Auth.Shared.Authorization;
using WingDing.Auth.Shared.Grpc;
using WingDing.Auth.Shared.Services;

namespace WingDing.Auth.Shared;

public static class DependencyInjection
{
    /// <summary>
    /// Registers [HasPermission] plumbing shared by ALL services:
    /// policy provider, requirement handler, claims transformation, AddAuthorization().
    /// Does NOT register IPermissionService — that's the caller's responsibility:
    ///   - AuthService registers LocalPermissionService (queries DB directly)
    ///   - Downstream services call AddWingDingAuthRemote which registers GrpcPermissionService
    /// Does NOT configure JWT — each service knows its own Keycloak setup.
    /// </summary>
    public static IServiceCollection AddWingDingAuthCore(this IServiceCollection services)
    {
        services.AddTransient<IClaimsTransformation, RemoteClaimsTransformation>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Used by downstream services (EventService, ClubService, UserService):
    /// JWT bearer + gRPC client + GrpcPermissionService bound to IPermissionService.
    /// Do NOT call from AuthService — it has its own JWT setup and LocalPermissionService.
    /// </summary>
    public static IServiceCollection AddWingDingAuthRemote(this IServiceCollection services,
        IConfiguration configuration,
        string authServiceGrpcUrl = "http://auth-service:5200")
    {
        var authSection = configuration.GetSection("Authentication");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Audience = authSection["Audience"];
                options.MetadataAddress = authSection["MetadataUrl"]!;
                options.RequireHttpsMetadata =
                    bool.Parse(authSection["RequireHttpsMetadata"] ?? "true");
                options.TokenValidationParameters.ValidIssuer = authSection["Issuer"];
            });

        services.AddMemoryCache();

        services.AddGrpcClient<PermissionOracle.PermissionOracleClient>(options =>
        {
            options.Address = new Uri(authServiceGrpcUrl);
        });

        services.AddScoped<IPermissionService, GrpcPermissionService>();

        return services;
    }
}
