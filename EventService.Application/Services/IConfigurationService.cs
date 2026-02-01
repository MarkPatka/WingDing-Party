using EventService.Application.Common.Configuration;

namespace EventService.Application.Services;

public interface IConfigurationService
{
    public EventsDatabaseConnection GetDatabaseInfo();
    public ApiSettings GetApiSettings();
    public PgAdminSettings GetPgAdminSettings();
}
