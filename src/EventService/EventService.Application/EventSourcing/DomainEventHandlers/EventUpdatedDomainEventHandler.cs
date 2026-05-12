using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Application.EventSourcing.Outbox;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class EventUpdatedDomainEventHandler
    : INotificationHandler<EventUpdated>
{
    private readonly IOutboxService _outbox;
    private readonly ILogger<EventUpdatedDomainEventHandler> _logger;

    public EventUpdatedDomainEventHandler(
        IOutboxService outbox,
        ILogger<EventUpdatedDomainEventHandler> logger)
    {
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(
        EventUpdated notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new EventUpdatedIntegrationEvent
        {
            EventId = notification.EventId.Value,
            UpdatedAt = notification.UpdatedAt
        };

        await _outbox.AddAsync(integrationEvent, cancellationToken);

        _logger.LogDebug(
            "Outbox: queued EventUpdated for EventId {EventId}",
            notification.EventId.Value);
    }
}
