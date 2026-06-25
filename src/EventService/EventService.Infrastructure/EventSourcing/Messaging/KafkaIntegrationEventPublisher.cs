using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public sealed class KafkaIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IEventProducer _producer;
    private readonly KafkaOptions _options;

    public KafkaIntegrationEventPublisher(
        IEventProducer producer, IOptions<KafkaOptions> options)
    {
        _producer = producer;
        _options = options.Value;
    }
    public Task PublishAsync<TEvent>(
        TEvent integrationEvent, CancellationToken cancellationToken = default) 
        where TEvent : IntegrationEvent
    {
        var key = integrationEvent.Id.ToString();
        return _producer.ProduceAsync(
            _options.ProduceEventsTopic, key, integrationEvent, cancellationToken);
    }
}
