using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class EventPublishedDomainEventHandler
    : INotificationHandler<EventPublished>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ILogger<EventPublishedDomainEventHandler> _logger;

    public EventPublishedDomainEventHandler(
        IIntegrationEventPublisher integrationEventPublisher,
        ILogger<EventPublishedDomainEventHandler> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
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

        await _integrationEventPublisher.PublishAsync(
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventPublished to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
