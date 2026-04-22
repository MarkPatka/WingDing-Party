using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using EventService.Domain.EventAggregate.DomainEvents;
using EventService.Infrastructure.EventSourcing.EventContracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.EventHandlers;

public sealed class EventCancelledDomainEventHandler
    : INotificationHandler<EventCancelled>
{
    private readonly IEventProducer _eventProducer;
    private readonly KafkaOptions _options;
    private readonly ILogger<EventCancelledDomainEventHandler> _logger;

    public EventCancelledDomainEventHandler(
        IEventProducer eventProducer,
        IOptions<KafkaOptions> options,
        ILogger<EventCancelledDomainEventHandler> logger)
    {
        _eventProducer = eventProducer;
        _options = options.Value;
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

        await _eventProducer.ProduceAsync(
            _options.ProduceEventsTopic,
            notification.EventId.Value.ToString(),
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventCancelled to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
