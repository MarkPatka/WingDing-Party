using EventService.Application.EventSourcing;

namespace EventService.Infrastructure.EventSourcing.EventContracts;

public sealed record EventPublishedIntegrationEvent : IntegrationEvent
{
    public Guid EventId { get; init; }
    public DateTime PublishedAt { get; init; }

    public EventPublishedIntegrationEvent()
    {
        EventType = nameof(EventPublishedIntegrationEvent);
    }
}
