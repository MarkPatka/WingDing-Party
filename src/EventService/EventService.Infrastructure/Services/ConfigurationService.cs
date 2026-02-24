using EventService.Application.Common.Configuration;
using EventService.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.Services;

internal class ConfigurationService : IConfigurationService
{
    private readonly EventsDatabaseOptions _dbConfig;
    private readonly ApiSettings _apiSettings;
    private readonly PgAdminSettings _pgAdminSettings;

    private readonly ILogger<ConfigurationService> _logger;


    // Three ways to inject configuration:
    // 1. IOptions<T>         - Singleton, configuration read once at startup
    // 2. IOptionsSnapshot<T> - Scoped, reloads configuration per request
    // 3. IOptionsMonitor<T>  - Singleton, reloads configuration on change
    public ConfigurationService(
        IOptions<EventsDatabaseOptions> dbConfig,
        IOptions<ApiSettings> apiSettings,
        IOptionsSnapshot<PgAdminSettings> pgAdminSettings,
        ILogger<ConfigurationService> logger)
    {
        _dbConfig = dbConfig.Value;
        _apiSettings = apiSettings.Value;
        _pgAdminSettings = pgAdminSettings.Value;

        _logger = logger;
    }

    public EventsDatabaseOptions GetDatabaseInfo()
    {
        _logger.LogInformation("Log proccess");
        _logger.LogWarning("Log smth strange");
        _logger.LogError("Errors");

        return _dbConfig;
    }
    public PgAdminSettings GetPgAdminSettings() => _pgAdminSettings;
    public ApiSettings GetApiSettings() => _apiSettings;
}
