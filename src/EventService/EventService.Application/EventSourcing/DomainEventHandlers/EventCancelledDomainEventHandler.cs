using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class EventCancelledDomainEventHandler
    : INotificationHandler<EventCancelled>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ILogger<EventCancelledDomainEventHandler> _logger;

    public EventCancelledDomainEventHandler(
        IIntegrationEventPublisher integrationEventPublisher,
        ILogger<EventCancelledDomainEventHandler> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
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

        await _integrationEventPublisher.PublishAsync(
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventCancelled to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
