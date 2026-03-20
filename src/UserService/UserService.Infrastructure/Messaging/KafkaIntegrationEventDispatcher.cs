using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.Common.Configuration;
using UserService.Application.IntegrationEvents;
using UserService.Infrastructure.Messaging.Extensions;

namespace UserService.Infrastructure.Messaging;

public class KafkaIntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private readonly ILogger<KafkaIntegrationEventDispatcher> _logger;
    private readonly IOptionsMonitor<Dictionary<string, KafkaOptions>> _options;
    private readonly IKafkaProducerFactory _producerFactory;

    public KafkaIntegrationEventDispatcher(IKafkaProducerFactory producerFactory,
        IOptionsMonitor<Dictionary<string, KafkaOptions>> options,
        ILogger<KafkaIntegrationEventDispatcher> logger)
    {
        ArgumentNullException.ThrowIfNull(producerFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _options = options;
        _logger = logger;
        _producerFactory = producerFactory;
    }

    public async Task DispatchAsync(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var aggregate = integrationEvent.GetAggregateName();
        var topic = _options.CurrentValue[aggregate].ProduceEventsTopic;
        if (string.IsNullOrWhiteSpace(topic))
        {
            _logger.LogError($"Topic name is missing for aggregate {aggregate}");
            throw new ArgumentNullException(nameof(topic));
        }

        var producer = _producerFactory.GetProducer(aggregate);
        var payload = JsonSerializer.Serialize(integrationEvent);

        await producer.ProduceAsync(topic,
            new Message<string, string> { Key = integrationEvent.Id.ToString(), Value = payload },
            cancellationToken);
    }

    public async Task DispatchToDeadLetterAsync(
        IIntegrationEvent integrationEvent,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var aggregate = integrationEvent.GetAggregateName();
        var dlqTopic = _options.CurrentValue[aggregate].DeadLetterTopic;
        if (string.IsNullOrWhiteSpace(dlqTopic))
        {
            return;
        }

        var deadLetter = new DeadLetterMessage
        {
            OriginalMessageId = integrationEvent.Id,
            OriginalType = integrationEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(
                integrationEvent,
                integrationEvent.GetType()),
            Error = exception.Message,
            StackTrace = exception.ToString(),
            FailedAtUtc = DateTime.UtcNow
        };

        var payload = JsonSerializer.Serialize(deadLetter);

        var producer = _producerFactory.GetProducer(aggregate);
        await producer.ProduceAsync(
            dlqTopic,
            new Message<string, string>
            {
                Key = integrationEvent.Id.ToString(),
                Value = payload,
                Headers = new Headers
                {
                    { "x-dead-letter", Encoding.UTF8.GetBytes("true") },
                    { "x-error-type", Encoding.UTF8.GetBytes(exception.GetType().Name) }
                }
            },
            cancellationToken);
    }
}