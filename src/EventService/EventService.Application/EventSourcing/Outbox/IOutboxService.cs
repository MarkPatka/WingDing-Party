namespace EventService.Application.EventSourcing.Outbox;

public interface IOutboxService
{
    Task AddAsync<TEvent>(
        TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;
}
