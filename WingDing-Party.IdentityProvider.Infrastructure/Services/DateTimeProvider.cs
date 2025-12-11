using WingDing_Party.IdentityProvider.Application.Common.Interfaces.Services;

namespace WingDing_Party.IdentityProvider.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => TimeProvider.System.GetLocalNow().DateTime;
    public DateTime UtcNow => TimeProvider.System.GetUtcNow().DateTime;
}
