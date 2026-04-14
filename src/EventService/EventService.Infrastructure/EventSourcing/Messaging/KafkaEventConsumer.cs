using Confluent.Kafka;
using EventService.Application.Common.Configuration;
using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventSourcing;
using EventService.Domain.EventAggregate.DomainEvents;
using EventService.Infrastructure.EventSourcing.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public record KafkaEventEnvelope(
    string EventType,
    string EventId,
    DateTime Timestamp,
    JsonElement Data);

public class KafkaEventConsumer : BackgroundService, IEventConsumer
{
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaEventConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDeadLetterQueueProducer _dlqProducer;

    private IConsumer<string, string>? _consumer;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public KafkaEventConsumer(
        IOptions<KafkaOptions> options,
        ILogger<KafkaEventConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IDeadLetterQueueProducer dlqProducer)
    {
        _options = options.Value;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _dlqProducer = dlqProducer;
    }

    public void Initialize()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 30000,
            HeartbeatIntervalMs = 10000
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetValueDeserializer(Deserializers.Utf8)
            .SetErrorHandler((_, e) => _logger.LogError(
                "Consumer error: {Reason}", e.Reason))
            .Build();

        _consumer.Subscribe(_options.ConsumeEventsTopic);
        _logger.LogInformation(
            "Consumer initialized. Subscribed to {Topics}",
            string.Join(", ", _options.ConsumeEventsTopic));
    }

    public async Task ConsumeMessageAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer?.Consume(stoppingToken);

                if (consumeResult == null)
                    continue;

                await ProcessMessageAsync(consumeResult, stoppingToken);

                _consumer?.Commit(consumeResult);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer cancelled");
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Consume error: {Error}", ex.Error.Reason);
            }
        }
    }

    public void Close()
    {
        _consumer?.Close();
        _logger.LogInformation("Consumer closed");
    }

    private async Task ProcessMessageAsync(
        ConsumeResult<string, string> result, 
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var envelope = JsonSerializer.Deserialize<KafkaEventEnvelope>(
                result.Message.Value, JsonOptions)!;

            _logger.LogInformation(
                "Processing {EventType} from {Topic}/{Partition}/{Offset}",
                envelope.EventType,
                result.Topic,
                result.Partition,
                result.Offset.Value);

            var domainEvent = envelope.MapToDomainEvent();
            var command = envelope.MapToCommand(); // DomainEvent -> Command

            await mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "Successfully processed {EventType}", envelope.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message from {Topic}/{Offset}",
                result.Topic, result.Offset.Value);

            await _dlqProducer.PublishAsync(
                originalTopic: result.Topic,
                messageKey: result.Message.Key,
                messageValue: result.Message.Value,
                errorReason: ex.Message,
                exception: ex,
                ct: cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Initialize();

        try
        {
            await ConsumeMessageAsync(stoppingToken);
        }
        finally
        {
            Close();
        }
    }

    public override void Dispose()
    {
        Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}