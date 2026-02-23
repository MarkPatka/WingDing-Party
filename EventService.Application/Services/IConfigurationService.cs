using EventService.Application.Common.Configuration;

namespace EventService.Application.Services;

public interface IConfigurationService
{
    public EventsDatabaseOptions GetDatabaseInfo();
    public ApiSettings GetApiSettings();
    public PgAdminSettings GetPgAdminSettings();
}
