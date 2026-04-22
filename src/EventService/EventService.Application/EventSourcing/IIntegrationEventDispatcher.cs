namespace EventService.Application.EventSourcing;

public interface IIntegrationEventDispatcher
{
    Task DispatchAsync(string eventType, string payload, CancellationToken cancellationToken);
}
