using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Application.EventSourcing.Outbox;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class EventDeletedDomainEventHandler
    : INotificationHandler<EventDeleted>
{
    private readonly IOutboxService _outbox;
    private readonly ILogger<EventDeletedDomainEventHandler> _logger;

    public EventDeletedDomainEventHandler(
        IOutboxService outbox,
        ILogger<EventDeletedDomainEventHandler> logger)
    {
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(
        EventDeleted notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new EventDeletedIntegrationEvent
        {
            EventId = notification.EventId.Value,
            DeletedAt = notification.DeletedAt
        };

        await _outbox.AddAsync(integrationEvent, cancellationToken);

        _logger.LogDebug(
            "Outbox: queued EventDeleted for EventId {EventId}",
            notification.EventId.Value);
    }
}
