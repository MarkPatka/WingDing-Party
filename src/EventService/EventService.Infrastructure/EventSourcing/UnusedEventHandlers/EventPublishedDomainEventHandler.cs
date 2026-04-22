using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using EventService.Domain.EventAggregate.DomainEvents;
using EventService.Infrastructure.EventSourcing.EventContracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.EventHandlers;

public sealed class EventPublishedDomainEventHandler
    : INotificationHandler<EventPublished>
{
    private readonly IEventProducer _eventProducer;
    private readonly KafkaOptions _options;
    private readonly ILogger<EventPublishedDomainEventHandler> _logger;

    public EventPublishedDomainEventHandler(
        IEventProducer eventProducer,
        IOptions<KafkaOptions> options,
        ILogger<EventPublishedDomainEventHandler> logger)
    {
        _eventProducer = eventProducer;
        _options = options.Value;
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

        await _eventProducer.ProduceAsync(
            _options.ProduceEventsTopic,
            notification.EventId.Value.ToString(),
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventPublished to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
