namespace EventService.Application.EventSourcing.IntegrationEvents.Outgoing;

public sealed record EventUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid EventId { get; init; }
    public DateTime UpdatedAt { get; init; }

    public EventUpdatedIntegrationEvent()
    {
        EventType = nameof(EventUpdatedIntegrationEvent);
    }
}
