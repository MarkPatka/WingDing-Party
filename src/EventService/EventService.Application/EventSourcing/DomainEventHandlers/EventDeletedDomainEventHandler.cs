using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class EventDeletedDomainEventHandler
    : INotificationHandler<EventDeleted>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ILogger<EventDeletedDomainEventHandler> _logger;

    public EventDeletedDomainEventHandler(
        IIntegrationEventPublisher integrationEventPublisher,
        ILogger<EventDeletedDomainEventHandler> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
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

        await _integrationEventPublisher.PublishAsync(
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventDeleted to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
