using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Services;

namespace WingDing_Party.AuthenticationService.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => TimeProvider.System.GetLocalNow().DateTime;
    public DateTime UtcNow => TimeProvider.System.GetUtcNow().DateTime;
}
