namespace EventService.Application.EventSourcing;

public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IntegrationEvent
{
    Task Handle(TEvent integrationEvent, CancellationToken cancellationToken);
}
