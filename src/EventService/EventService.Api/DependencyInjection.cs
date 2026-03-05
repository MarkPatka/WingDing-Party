using Mapster;
using MapsterMapper;
using EventService.Api.Middleware.GlobalErrorHandler;
using EventService.Application.Common.Configuration;
using Serilog;
using Serilog.Events;
using System.Reflection;
using Microsoft.Extensions.Configuration.Memory;

namespace EventService.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddLogging()
            .AddConfiguration(configuration)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddErrorHandler()
            .AddMappings()
            .AddControllers();

        return services;
    }

    private static IServiceCollection AddErrorHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
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

    private static IServiceCollection AddConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        LoadEnvironmentVariables();

        configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>();

        services.BindConfigurations(configuration);

        return services;
    }

    private static IServiceCollection BindConfigurations(this IServiceCollection services, ConfigurationManager configuration)
    {
        // bind appsettings
        services.Configure<ApiSettings>(
            configuration.GetSection(ApiSettings.SectionName));

        services.Configure<PgAdminSettings>(
            configuration.GetSection(PgAdminSettings.SectionName));

        // bind .env
        var EventsDatabaseOptionsConfig = new ConfigurationBuilder()
            .Add(new MemoryConfigurationSource { InitialData = GetEnvVarsForClass<EventsDatabaseOptions>() })
            .Build();

        services.Configure<EventsDatabaseOptions>(EventsDatabaseOptionsConfig);

        // validate settings
        services.AddOptions<ApiSettings>()
            .Validate(x => x.Port > 0, "API Port must be greater than 0")
            .ValidateOnStart();

        services.AddOptions<EventsDatabaseOptions>()
            .Validate(x => !string.IsNullOrEmpty(x.CONNECTION_STRING), "Connection string is required")
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Logger(l =>
            {
                l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                .WriteTo.File(
                    path: "../logs/Information/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            })            
            .WriteTo.Logger(l =>
            {
                l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                .WriteTo.File(
                    path: "../logs/Warning/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            })
            .WriteTo.Logger(l =>
            {
                l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                .WriteTo.File(
                    path: "../logs/Error/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            })
            .CreateLogger();

        Log.Logger = logger;
        services.AddSerilog(logger);

        return services;
    }

    private static void LoadEnvironmentVariables()
    {
        var envPath = Path.Combine(
            Directory.GetCurrentDirectory(), "..", ".env");

        if (File.Exists(envPath))
        {
            Log.Information("Loading environment variables from: {0}", envPath);

            DotNetEnv.Env.Load(envPath);

            Log.Information("Environment variables loaded successfully");
        }
        else
        {
            Log.Warning(".env file not found at: {0}", envPath);
        }
    }

    private static Dictionary<string, string?> GetEnvVarsForClass<T>()
    {
        return typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite)
            .ToDictionary(
                p => p.Name,
                p => Environment.GetEnvironmentVariable(p.Name),
                StringComparer.OrdinalIgnoreCase);
    }
}

