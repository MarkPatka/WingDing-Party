namespace EventService.Application.Services;

public interface ITimeProviderService
{
    public DateTime Now { get; }
    public DateTime UtcNow { get; }
}
