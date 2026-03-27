using Confluent.Kafka;
using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public class KafkaEventProducer : IEventProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaEventProducer> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public KafkaEventProducer(
        IOptions<KafkaOptions> options,
        ILogger<KafkaEventProducer> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _options = options.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageTimeoutMs = 5000,
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
                _logger.LogError(e.Reason, "Kafka producer error"))
            .SetLogHandler((_, log) =>
                _logger.LogDebug("Kafka: {Message}", log.Message))
            .Build();
    }

    public async Task ProduceAsync<TMessage>(
        string topic, 
        string key, 
        TMessage message, 
        CancellationToken cancellationToken = default) 
        where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(message);

        var payload = JsonSerializer.Serialize(message, JsonOptions);

        try
        {
            var result = await _producer.ProduceAsync(
                topic,
                new Message<string, string> { Key = key, Value = payload},
                cancellationToken);

            _logger.LogDebug(
                "Produced message to {Topic} partition [{Partition}] offset {Offset}",
                result.Topic,
                result.Partition.Value,
                result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(
                ex,
                "Failed to produce message to {Topic}. Sending to DLQ.",
                topic);
            await SendToDeadLetterQueueAsync(key, payload, ex, cancellationToken);
            throw;
        }
    }

    private async Task SendToDeadLetterQueueAsync(
        string key, 
        string payload, 
        ProduceException<string, string> originalException, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.DeadLetterTopic))
        {
            _logger.LogWarning(
                "DeadLetterTopic is not configured; skipping DLQ send.");
            return;
        }

        try
        {
            var headers = new Headers
            {
                {
                    "dlq-error",
                    Encoding.UTF8.GetBytes(
                        originalException.Error?.Reason 
                        ?? originalException.Message)
                },
                {
                    "dlq-timestamp",
                    Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O"))
                }
            };

            var dlqMessage = new Message<string, string>
            {
                Key = key,
                Value = payload,
                Headers = headers
            };

            await _producer.ProduceAsync(
                _options.DeadLetterTopic,
                dlqMessage,
                cancellationToken);

            _logger.LogInformation(
                "Message sent to DLQ topic {DlqTopic} for key {Key}",
                _options.DeadLetterTopic,
                key);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send message to DLQ topic {DlqTopic}. " +
                "Original error: {OriginalError}",
                _options.DeadLetterTopic,
                originalException.Message);
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }
}
