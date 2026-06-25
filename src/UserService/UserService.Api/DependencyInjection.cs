using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using UserService.Api.Middleware.GlobalErrorHandler;
using UserService.Application.Common.Configuration;
using WingDing.Auth.Shared;

namespace UserService.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services
            .AddLogging()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddConfiguration(configuration)
            .AddWingDingAuthCore()
            .AddWingDingAuthRemote(configuration)
            .AddErrorHandler();
        
        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        LoadEnvironmentVariables();
        
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
        // bind minio settings
        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));

        // bind kafka settings
        services.AddOptions<Dictionary<string, KafkaOptions>>()
            .Bind(configuration.GetSection(KafkaOptions.SectionName));

        // bind api settings
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));

        // bind .env
        services.Configure<UserDatabaseOptions>(options => configuration.Bind(options));

        // validate settings
        services.AddOptions<ApiOptions>()
            .Validate(x => x.Port > 0, "API Port must be greater than 0")
            .ValidateOnStart();

        services.AddOptions<UserDatabaseOptions>()
            .Validate(x => !string.IsNullOrEmpty(x.CONNECTION_STRING), "Connection string is required")
            .ValidateOnStart();

        return services;
    }


    private static IServiceCollection AddErrorHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
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

    private static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        // TODO: DocInclusionPredicate and Groups with OperationFilter
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
    private static void LoadEnvironmentVariables()
    {
        var envPath = Path.Combine(
            Directory.GetCurrentDirectory(), "..", ".env");

        if (File.Exists(envPath))
        {
            Log.Information("Loading environment variables from: {EnvPath}", envPath);

            DotNetEnv.Env.Load(envPath);

            Log.Information("Environment variables loaded successfully");
        }
        else
        {
            Log.Warning(".env file not found at: {EnvPath}", envPath);
        }
    }
}