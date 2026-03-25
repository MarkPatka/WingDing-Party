using EventService.Application.EventSourcing;

namespace EventService.Infrastructure.EventSourcing.EventContracts;

public sealed record EventUpdatedIntegrationEvent : IntergrationEvent
{
    public Guid EventId { get; init; }
    public DateTime UpdatedAt { get; init; }

    public EventUpdatedIntegrationEvent()
    {
        EventType = nameof(EventUpdatedIntegrationEvent);
    }
}
