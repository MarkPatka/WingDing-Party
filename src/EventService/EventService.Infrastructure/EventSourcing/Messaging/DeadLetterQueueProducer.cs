using Confluent.Kafka;
using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public record DeadLetterEnvelope(
    string OriginalTopic,
    string Key,
    string Value,
    string ErrorReason,
    string? ExceptionType,
    string? ExceptionMessage,
    string? StackTrace,
    DateTimeOffset Timestamp);

public class DeadLetterQueueProducer : IDeadLetterQueueProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _options;
    private readonly ILogger<DeadLetterQueueProducer> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public DeadLetterQueueProducer(
        IOptions<KafkaOptions> options,
        ILogger<DeadLetterQueueProducer> logger)
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
                _logger.LogError("DLQ producer error: {Reason}", e.Reason))
            .SetLogHandler((_, log) =>
                _logger.LogDebug("DLQ Kafka: {Message}", log.Message))
            .Build();
    }

    public async Task PublishAsync(
        string originalTopic, 
        string? messageKey, 
        string? messageValue, 
        string errorReason, 
        Exception? exception = null, 
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalTopic);
        ArgumentException.ThrowIfNullOrWhiteSpace(errorReason);

        if (string.IsNullOrWhiteSpace(_options.DeadLetterTopic))
        {
            _logger.LogWarning(
                "DeadLetterTopic not configured; skipping DLQ publish.");
            return;
        }

        var envelope = new DeadLetterEnvelope(
            OriginalTopic: originalTopic,
            Key: messageKey ?? string.Empty,
            Value: messageValue ?? string.Empty,
            ErrorReason: errorReason,
            ExceptionType: exception?.GetType().FullName,
            ExceptionMessage: exception?.Message ?? string.Empty,
            StackTrace: exception?.StackTrace,
            Timestamp: DateTimeOffset.UtcNow
        );

        var payload = JsonSerializer.Serialize(envelope, JsonOptions);

        try
        {
            var dlqKey = $"dlq-{messageKey ?? Guid.NewGuid().ToString()}";

            var headers = new Headers
            {
                {
                    "dlq-original-topic",
                    Encoding.UTF8.GetBytes(originalTopic)
                },
                {
                    "dlq-error-reason",
                    Encoding.UTF8.GetBytes(errorReason)
                },
                {
                    "dlq-timestamp",
                    Encoding.UTF8.GetBytes(envelope.Timestamp.ToString("O"))
                }
            };

            if (exception != null)
            {
                headers.Add(
                    "dlq-exception-type",
                    Encoding.UTF8.GetBytes(exception.GetType().Name)
                );
            }

            var dlqMessage = new Message<string, string>
            {
                Key = dlqKey,
                Value = payload,
                Headers = headers
            };

            var result = await _producer.ProduceAsync(
                _options.DeadLetterTopic,
                dlqMessage,
                ct);

            _logger.LogInformation(
                "Published to DLQ {DlqTopic}/{Partition}/{Offset}: " +
                "topic={OriginalTopic}, key={Key}",
                _options.DeadLetterTopic,
                result.Partition.Value,
                result.Offset.Value,
                originalTopic,
                dlqKey);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogCritical(
                ex,
                "CRITICAL: Failed to publish to DLQ {DlqTopic}. " +
                "Message lost! {Error}",
                _options.DeadLetterTopic,
                ex.Error.Reason);
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
