using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using EventService.Domain.EventAggregate.DomainEvents;
using EventService.Infrastructure.EventSourcing.EventContracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.EventHandlers;

public sealed class EventDeletedDomainEventHandler
    : INotificationHandler<EventDeleted>
{
    private readonly IEventProducer _eventProducer;
    private readonly KafkaOptions _options;
    private readonly ILogger<EventDeletedDomainEventHandler> _logger;

    public EventDeletedDomainEventHandler(
        IEventProducer eventProducer,
        IOptions<KafkaOptions> options,
        ILogger<EventDeletedDomainEventHandler> logger)
    {
        _eventProducer = eventProducer;
        _options = options.Value;
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

        await _eventProducer.ProduceAsync(
            _options.ProduceEventsTopic,
            notification.EventId.Value.ToString(),
            integrationEvent,
            cancellationToken);

        _logger.LogDebug(
            "Published EventDeleted to Kafka for EventId {EventId}",
            notification.EventId.Value);
    }
}
