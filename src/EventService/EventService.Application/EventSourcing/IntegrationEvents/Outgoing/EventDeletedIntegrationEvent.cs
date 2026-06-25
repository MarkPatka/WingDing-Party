namespace EventService.Application.EventSourcing.IntegrationEvents.Outgoing;

public sealed record EventDeletedIntegrationEvent : IntegrationEvent
{
    public Guid EventId { get; init; }
    public DateTime DeletedAt { get; init; }

    public EventDeletedIntegrationEvent()
    {
        EventType = nameof(EventDeletedIntegrationEvent);
    }
}
