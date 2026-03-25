using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using EventService.Domain.EventAggregate.DomainEvents;
using EventService.Infrastructure.EventSourcing.EventContracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.EventHandlers;

public sealed class EventCreatedDomainEventHandler 
    : INotificationHandler<EventCreated>
{
    private readonly IEventProducer _eventProducer;
    private readonly KafkaOptions _options;
    private readonly ILogger<EventCreatedDomainEventHandler> _logger;

    public EventCreatedDomainEventHandler(
        IEventProducer eventProducer,
        IOptions<KafkaOptions> options,
        ILogger<EventCreatedDomainEventHandler> logger)
    {
        _eventProducer = eventProducer;
        _options = options.Value;
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
