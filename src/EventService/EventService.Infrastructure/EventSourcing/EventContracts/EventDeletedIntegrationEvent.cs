using EventService.Application.EventSourcing;

namespace EventService.Infrastructure.EventSourcing.EventContracts;

public sealed record EventDeletedIntegrationEvent : IntergrationEvent
{
    public Guid EventId { get; init; }
    public DateTime DeletedAt { get; init; }

    public EventDeletedIntegrationEvent()
    {
        EventType = nameof(EventDeletedIntegrationEvent);
    }
}
