using EventService.Application.Services;

namespace EventService.Infrastructure.Services;

public class TimeProviderService
    : ITimeProviderService
{
    public DateTime Now => TimeProvider.System.GetLocalNow().DateTime;
    public DateTime UtcNow => TimeProvider.System.GetUtcNow().DateTime;
}
