using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;
using EventService.Application.EventSourcing.Outbox;
using EventService.Domain.EventAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.DomainEventHandlers;

public sealed class ParticipantRegisteredDomainEventHandler
    : INotificationHandler<ParticipantRegistered>
{
    private readonly IOutboxService _outbox;
    private readonly ILogger<ParticipantRegisteredDomainEventHandler> _logger;

    public ParticipantRegisteredDomainEventHandler(
        IOutboxService outbox,
        ILogger<ParticipantRegisteredDomainEventHandler> logger)
    {
        _outbox = outbox;
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

        await _outbox.AddAsync(integrationEvent, cancellationToken);

        _logger.LogDebug(
            "Outbox: queued ParticipantRegistered for EventId {EventId} with ParticipantId {ParticipantId}",
            notification.EventId.Value,
            notification.ParticipantId.Value);
    }
}
