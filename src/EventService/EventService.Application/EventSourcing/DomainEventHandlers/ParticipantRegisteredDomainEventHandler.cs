using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class ParticipantRegisteredDomainEventHandler
    : INotificationHandler<ParticipantRegistered>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ILogger<ParticipantRegisteredDomainEventHandler> _logger;

    public ParticipantRegisteredDomainEventHandler(
        IIntegrationEventPublisher integrationEventPublisher,
        ILogger<ParticipantRegisteredDomainEventHandler> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _logger = logger;
    }

    public async Task Handle(
        ParticipantRegistered notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ParticipantRegisteredIntegrationEvent
        {
            EventId = notification.EventId.Value,
            ParticipantId = notification.ParticipantId.Value,
            RegisteredAt = notification.RegisteredAt
        };

        await _integrationEventPublisher.PublishAsync(
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published ParticipantRegistered to Kafka for EventId {EventId} with ParticipantId {ParticipantId}",
            notification.EventId.Value,
            notification.ParticipantId.Value);
    }
}
