using EventService.Application.EventSourcing;
using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Infrastructure.EventSourcing.EventHandlers;

public sealed class EventUpdatedDomainEventHandler
    : INotificationHandler<EventUpdated>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ILogger<EventUpdatedDomainEventHandler> _logger;

    public EventUpdatedDomainEventHandler(
        IIntegrationEventPublisher integrationEventPublisher,
        ILogger<EventUpdatedDomainEventHandler> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
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

        await _integrationEventPublisher.PublishAsync(
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventCreated to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
