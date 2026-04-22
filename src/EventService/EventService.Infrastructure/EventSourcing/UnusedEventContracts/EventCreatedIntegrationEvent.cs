using EventService.Application.EventSourcing;

namespace EventService.Infrastructure.EventSourcing.EventContracts;

public sealed record EventCreatedIntegrationEvent : IntegrationEvent
{
    public Guid EventId { get; init; }
    public DateTime CreatedAt { get; init; }

    public EventCreatedIntegrationEvent()
    {
        EventType = nameof(EventCreatedIntegrationEvent);
    }
}
