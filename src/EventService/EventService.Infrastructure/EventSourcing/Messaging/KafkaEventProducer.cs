using Confluent.Kafka;
using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public class KafkaEventProducer : IEventProducer
{
    private readonly IProducer<string, string> _producer;
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

        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageTimeoutMs = 5000,
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
                _logger.LogError("Kafka producer error: {Reason}", e.Reason))
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
                "Failed to produce message to {Topic} for key {Key}",
                topic, key);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }
}
