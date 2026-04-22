using EventService.Application.EventSourcing;
using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Infrastructure.EventSourcing.EventHandlers;

public sealed class EventCreatedDomainEventHandler 
    : INotificationHandler<EventCreated>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ILogger<EventCreatedDomainEventHandler> _logger;

    public EventCreatedDomainEventHandler(
        IIntegrationEventPublisher integrationEventPublisher,
        ILogger<EventCreatedDomainEventHandler> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
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

        await _integrationEventPublisher.PublishAsync(
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventCreated to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
