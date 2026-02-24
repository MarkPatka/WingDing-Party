using System.Collections.Concurrent;
using ClubService.Application.Common.Configuration;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClubService.Infrastructure.Messaging;

public class KafkaProducerFactory : IKafkaProducerFactory, IDisposable
{
    private readonly ConcurrentDictionary<string, IProducer<string, string>> _cache = new();
    private readonly IOptionsMonitor<Dictionary<string, KafkaOptions>> _options;
    private readonly ILogger<KafkaProducerFactory> _logger;

    public KafkaProducerFactory(IOptionsMonitor<Dictionary<string, KafkaOptions>> options,
        ILogger<KafkaProducerFactory> logger)
    {
        _options = options;
        _logger = logger;
    }

    public IProducer<string, string> GetProducer(string aggregate)
    {
        return _cache.GetOrAdd(aggregate, CreateProducer);
    }

    private IProducer<string, string> CreateProducer(string aggregate)
    {
        if (!_options.CurrentValue.TryGetValue(aggregate, out var cfg))
            throw new InvalidOperationException($"Kafka config '{aggregate}' not found");

        var config = new ProducerConfig
        {
            BootstrapServers = cfg.BootstrapServers,
            Acks = (Acks)cfg.Acks,
            EnableIdempotence = cfg.EnableIdempotence,
            MessageTimeoutMs = cfg.MessageTimeoutMs
        };
        return new ProducerBuilder<string, string>(config).SetErrorHandler((_, e) => _logger.LogError(e.Reason))
            .Build();
    }

    public void Dispose()
    {
        foreach (var p in _cache.Values) p.Dispose();
    }
}