using AuthService.Api.Middleware.GlobalErrorHandler;
using Mapster;
using MapsterMapper;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using System.Reflection;

namespace AuthService.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services
            .AddLogging()
            .AddMappings()
            .AddEndpointsApiExplorer()
            .AddOpenApi()
            .AddSwaggerGen()
            .AddConfiguration(configuration)
            .AddErrorHandler()
            .AddGrpc()
            ;

        // MapControllers() in Program.cs requires the MVC controller services
        services.AddControllers();

        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>();

        return services;
    }


    private static IServiceCollection AddErrorHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT токен от Keycloak",
                Name = "Authorization",
                In = ParameterLocation.Header
            });

            options.AddSecurityRequirement(doc =>
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", doc),
                        new List<string>()
                    }
                });
        });

        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }

    public static IServiceCollection AddLogging(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.Logger(l =>
            {
                l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                    .WriteTo.File(
                        path: "../logs/Information/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate:
                        "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            })
            .WriteTo.Logger(l =>
            {
                l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                    .WriteTo.File(
                        path: "../logs/Warning/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate:
                        "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            })
            .WriteTo.Logger(l =>
            {
                l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                    .WriteTo.File(
                        path: "../logs/Error/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate:
                        "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            })
            .CreateLogger();

        Log.Logger = logger;
        services.AddSerilog(logger);

        return services;
    }

}
