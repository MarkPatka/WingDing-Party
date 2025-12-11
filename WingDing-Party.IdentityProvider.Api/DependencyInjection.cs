using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi;
using System.Reflection;
using WingDing_Party.IdentityProvider.Api.Common.Errors;

namespace WingDing_Party.IdentityProvider.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddSwagger()
            .AddGlobalErrorHandling()
            .AddMappings();

        return services;
    }

    private static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
    {
        services.AddSingleton<ProblemDetailsFactory, IdentityProviderProblemDetailsFactory>();
        services.AddProblemDetails();
        return services;
    }

    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WingDing Authentication Service Api",
                Version = "v1",
                Description = "Example API with Swagger"
            });
        });
        return services;
    }
}
