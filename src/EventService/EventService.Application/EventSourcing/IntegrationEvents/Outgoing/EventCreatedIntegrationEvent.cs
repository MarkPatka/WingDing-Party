namespace EventService.Application.EventSourcing.IntegrationEvents.Outgoing;

public sealed record EventCreatedIntegrationEvent : IntegrationEvent
{
    public Guid EventId { get; init; }
    public DateTime CreatedAt { get; init; }

    public EventCreatedIntegrationEvent()
    {
        EventType = nameof(EventCreatedIntegrationEvent);
    }
}
