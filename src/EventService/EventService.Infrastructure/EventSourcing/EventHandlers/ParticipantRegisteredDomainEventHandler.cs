using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using EventService.Domain.EventAggregate.DomainEvents;
using EventService.Infrastructure.EventSourcing.EventContracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.EventHandlers;

public sealed class ParticipantRegisteredDomainEventHandler
    : INotificationHandler<ParticipantRegistered>
{
    private readonly IEventProducer _eventProducer;
    private readonly KafkaOptions _options;
    private readonly ILogger<ParticipantRegisteredDomainEventHandler> _logger;

    public ParticipantRegisteredDomainEventHandler(
        IEventProducer eventProducer,
        IOptions<KafkaOptions> options,
        ILogger<ParticipantRegisteredDomainEventHandler> logger)
    {
        _eventProducer = eventProducer;
        _options = options.Value;
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

        await _eventProducer.ProduceAsync(
            _options.ProduceEventsTopic,
            notification.EventId.Value.ToString(),
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published ParticipantRegistered to Kafka for EventId {EventId} with ParticipantId {ParticipantId}",
            notification.EventId.Value,
            notification.ParticipantId.Value);
    }
}
