using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Application.EventSourcing.Outbox;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class EventCreatedDomainEventHandler 
    : INotificationHandler<EventCreated>
{
    private readonly IOutboxService _outbox;
    private readonly ILogger<EventCreatedDomainEventHandler> _logger;

    public EventCreatedDomainEventHandler(
        IOutboxService outbox,
        ILogger<EventCreatedDomainEventHandler> logger)
    {
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(
        EventCreated notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new EventCreatedIntegrationEvent
        {
            EventId = notification.EventId.Value,
            CreatedAt = notification.CreatedAt
        };

        await _outbox.AddAsync(integrationEvent, cancellationToken);

        _logger.LogDebug(
            "Outbox: queued EventCreated for EventId {EventId}",
            notification.EventId.Value);
    }
}
