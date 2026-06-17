namespace EventService.Application.EventSourcing.IntegrationEvents.Outgoing;

public sealed record EventPublishedIntegrationEvent : IntegrationEvent
{
    public Guid EventId { get; init; }
    public DateTime PublishedAt { get; init; }

    public EventPublishedIntegrationEvent()
    {
        EventType = nameof(EventPublishedIntegrationEvent);
    }
}
