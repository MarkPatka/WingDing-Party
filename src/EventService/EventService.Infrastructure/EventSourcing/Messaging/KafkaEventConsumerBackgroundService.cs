using EventService.Application.EventSourcing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public sealed class KafkaEventConsumerBackgroundService : BackgroundService
{
    private readonly IEventConsumer _consumer;
    private readonly ILogger<KafkaEventConsumerBackgroundService> _logger;

    public KafkaEventConsumerBackgroundService(
        IEventConsumer consumer,
        ILogger<KafkaEventConsumerBackgroundService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        try
        {
            _consumer.Initialize();
            await _consumer.ConsumeMessageAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consume message operation is cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Consumer background service terminated.");
        }
        finally
        {
            _consumer.Close();
        }
    }
}
