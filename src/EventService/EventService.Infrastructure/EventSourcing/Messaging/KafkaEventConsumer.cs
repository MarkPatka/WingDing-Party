using Confluent.Kafka;
using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public sealed class KafkaEventConsumer : IEventConsumer
{
    private readonly KafkaOptions _options;
    private readonly IIntegrationEventDispatcher _dispatcher;
    private readonly IDeadLetterQueueProducer _dlqProducer;
    private readonly ResiliencePipeline _retryPipeline;
    private readonly ILogger<KafkaEventConsumer> _logger;

    private IConsumer<string, string>? _consumer;
    private bool _disposed;

    public KafkaEventConsumer(
        IOptions<KafkaOptions> options,
        IIntegrationEventDispatcher dispatcher,
        IDeadLetterQueueProducer dlqProducer,
        ILogger<KafkaEventConsumer> logger)
    {
        _options = options.Value;
        _logger = logger;
        _dispatcher = dispatcher;
        _dlqProducer = dlqProducer;

        _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = _options.MaxRetryAttempts,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(_options.RetryDelayMs),
                ShouldHandle = new PredicateBuilder()
                    .Handle<Exception>(ex => ex is not OperationCanceledException),
                OnRetry = args =>
                {
                    _logger.LogWarning(args.Outcome.Exception,
                        "Retry {Attempt}/{Max} after {Delay}ms",
                        args.AttemptNumber + 1,
                        _options.MaxRetryAttempts,
                        args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public void Initialize()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.ConsumerGroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_options.AutoOffsetReset, true),
            EnableAutoCommit = false,
            SessionTimeoutMs = 10_000,
            HeartbeatIntervalMs = 3_000,
            MaxPollIntervalMs = 300_000,
            EnablePartitionEof = true
        };

        if (string.IsNullOrWhiteSpace(_options.ConsumerGroupId))
            throw new InvalidOperationException("ConsumerGroupId is required");

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => 
                _logger.LogError("Consumer error: {Reason}", e.Reason))
            .SetPartitionsAssignedHandler((_, ps) =>
                _logger.LogInformation("Partitions assigned: {P}",
                    string.Join(",", ps)))
            .SetPartitionsRevokedHandler((_, ps) =>
                _logger.LogInformation("Partitions revoked: {P}",
                    string.Join(",", ps)))
            .Build();

        _consumer.Subscribe(_options.ConsumeEventsTopic);

        _logger.LogInformation(
            "Consumer ready. Group={Group}, Topics=[{Topics}]",
            _options.ConsumerGroupId,
            string.Join(", ", _options.ConsumeEventsTopic));
    }

    public async Task ConsumeMessageAsync(CancellationToken stoppingToken)
    {
        ThrowIfDisposed();

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<string, string>? result = null;
            try
            {
                result = _consumer?.Consume(stoppingToken);

                if (result is null || result.IsPartitionEOF)
                    continue;

                if (result.Message?.Value is null)
                {
                    _logger.LogWarning("Null message at {Topic}/{Offset}, commiting.",
                        result.Topic, result.Offset.Value);
                    _consumer?.Commit(result);
                    continue;
                }

                var processed = await TryProcessAsync(result, stoppingToken);
                if (processed)
                    _consumer?.Commit(result);
            }
            catch (ConsumeException ex) when (ex.Error.IsFatal)
            {
                _logger.LogCritical(ex, "Fatal consume error. Shutting down.");
                throw;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Consume error: {Reason}", ex.Error.Reason);
                await Task.Delay(1_000, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Consumer received shutdown signal");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected loop error.");
                await Task.Delay(1_000, stoppingToken);
            }
        }
    }

    private async Task<bool> TryProcessAsync(
        ConsumeResult<string, string> result, 
        CancellationToken ct)
    {
        try
        {
            await _retryPipeline.ExecuteAsync(async token =>
            {
                var eventType = ExtractEventType(result.Message.Value);
                await _dispatcher.DispatchAsync(eventType, result.Message.Value, token);
            }, ct);

            return true;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Exhausted retries for {Topic}/{Offset}. Routing to DLQ.",
                result.Topic, result.Offset.Value);

            await _dlqProducer.PublishAsync(
                originalTopic: result.Topic,
                messageKey: result.Message.Key,
                messageValue: result.Message.Value,
                errorReason: $"Exhausted {_options.MaxRetryAttempts} retries",
                exception: ex,
                ct: ct);

            return true;
        }
    }

    private static string ExtractEventType(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("eventType", out var prop)
            || prop.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException("Message has no 'eventType' discriminator.");

        return prop.GetString()!;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(KafkaEventConsumer));
    }

    public void Close()
    {
        try
        {
            _consumer?.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing consumer.");
        }

        _logger.LogInformation("Consumer closed");
    }

    public void Dispose()
    {
        if (_disposed) return;
        Close();
        _consumer?.Dispose();
        _consumer = null;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}