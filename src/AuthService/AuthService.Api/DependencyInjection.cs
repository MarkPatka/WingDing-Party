using AuthService.Infrastructure.Common.Configuration;
using Mapster;
using MapsterMapper;
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
            .AddErrorHandler();
        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        // LoadEnvironmentVariables();

        configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>();

        services.BindConfigurations(configuration);


        return services;
    }

    private static IServiceCollection BindConfigurations(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        // bind .env
        services.Configure<AuthDatabaseOptions>(configuration.Bind);
        services.Configure<AuthenticationOptions>(configuration.Bind); 
        services.Configure<KeycloakOptions>(configuration.Bind);
        
        // validate settings

        return services;
    }


    private static IServiceCollection AddErrorHandler(this IServiceCollection services)
    {
        //services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

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
