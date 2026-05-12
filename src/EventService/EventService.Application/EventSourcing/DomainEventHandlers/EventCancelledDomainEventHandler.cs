using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Application.EventSourcing.Outbox;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class EventCancelledDomainEventHandler
    : INotificationHandler<EventCancelled>
{
    private readonly IOutboxService _outbox;
    private readonly ILogger<EventCancelledDomainEventHandler> _logger;

    public EventCancelledDomainEventHandler(
        IOutboxService outbox,
        ILogger<EventCancelledDomainEventHandler> logger)
    {
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(
        EventCancelled notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new EventCancelledIntegrationEvent
        {
            EventId = notification.EventId.Value,
            CancelledAt = notification.CancelledAt
        };

        await _outbox.AddAsync(integrationEvent, cancellationToken);

        _logger.LogDebug(
            "Outbox: queued EventCancelled for EventId {EventId}",
            notification.EventId.Value);
    }
}
