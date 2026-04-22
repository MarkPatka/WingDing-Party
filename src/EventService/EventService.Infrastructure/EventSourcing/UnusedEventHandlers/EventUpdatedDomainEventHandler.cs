using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using EventService.Domain.EventAggregate.DomainEvents;
using EventService.Infrastructure.EventSourcing.EventContracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.EventHandlers;

public sealed class EventUpdatedDomainEventHandler
    : INotificationHandler<EventUpdated>
{
    private readonly IEventProducer _eventProducer;
    private readonly KafkaOptions _options;
    private readonly ILogger<EventUpdatedDomainEventHandler> _logger;

    public EventUpdatedDomainEventHandler(
        IEventProducer eventProducer,
        IOptions<KafkaOptions> options,
        ILogger<EventUpdatedDomainEventHandler> logger)
    {
        _eventProducer = eventProducer;
        _options = options.Value;
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

        await _eventProducer.ProduceAsync(
            _options.ProduceEventsTopic,
            notification.EventId.Value.ToString(),
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventCreated to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
