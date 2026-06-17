using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Outbox;

internal sealed class OutboxProcessor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly EventServiceDbContext _context;
    private readonly IIntegrationEventPublisher _publisher;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        EventServiceDbContext context,
        IIntegrationEventPublisher publisher,
        IOptions<OutboxOptions> options,
        ILogger<OutboxProcessor> logger)
    {
        _context = context;
        _publisher = publisher;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<int> ProcessBatchAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _context.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null
                    && x.RetryCount < _options.MaxRetryAttempts)
            .OrderBy(x => x.OccuredOnUtc)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
            return 0;

        _logger.LogInformation(
            "Processing {Count} outbox messages", messages.Count);

        var processed = 0;

        foreach (var message in messages)
        {
            try
            {
                await PublishAsync(message, cancellationToken);

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
                processed++;

                _logger.LogDebug(
                    "Published outbox message: Type={Type}, Id={Id}",
                    message.Type, message.Id);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message.Length > 2000
                    ? ex.Message[..2000] 
                    : ex.Message;

                _logger.LogError(ex,
                    "Failed to publish outbox message: Type {Type}, Id={Id}, Attempt={Attempt}/{Max}",
                    message.Type, message.Id, message.RetryCount, _options.MaxRetryAttempts);

                if (message.RetryCount >= _options.MaxRetryAttempts)
                {
                    _logger.LogCritical(
                        "Outbox message {Id} exhausted retries - manual intervention required",
                        message.Id);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return processed;
    }

    private async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var type = OutgoingTypes.Resolve(message.Type)
            ?? throw new InvalidOperationException(
                $"Unknown outbox message type: {message.Type}");

        var integrationEvent = (IntegrationEvent?)JsonSerializer.Deserialize(
            message.Content, type, JsonOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize outbox message {message.Id}");

        await _publisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
