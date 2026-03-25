using EventService.Application.EventSourcing;

namespace EventService.Infrastructure.EventSourcing.EventContracts;

public sealed record EventCancelledIntegrationEvent : IntergrationEvent
{
    public Guid EventId { get; init; }
    public DateTime CancelledAt { get; init; }

    public EventCancelledIntegrationEvent()
    {
        EventType = nameof(EventCancelledIntegrationEvent);
    }
}
