using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Application.EventSourcing.Outbox;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class EventPublishedDomainEventHandler
    : INotificationHandler<EventPublished>
{
    private readonly IOutboxService _outbox;
    private readonly ILogger<EventPublishedDomainEventHandler> _logger;

    public EventPublishedDomainEventHandler(
        IOutboxService outbox,
        ILogger<EventPublishedDomainEventHandler> logger)
    {
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(
        EventPublished notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new EventPublishedIntegrationEvent
        {
            EventId = notification.EventId.Value,
            PublishedAt = notification.OccurredAt
        };

        await _outbox.AddAsync(integrationEvent, cancellationToken);

        _logger.LogDebug(
            "Outbox: queued EventPublished for EventId {EventId}",
            notification.EventId.Value);
    }
}
