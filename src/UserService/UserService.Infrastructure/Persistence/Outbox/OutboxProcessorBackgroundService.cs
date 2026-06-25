using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserService.Application.IntegrationEvents;
using UserService.Infrastructure.Messaging;

namespace UserService.Infrastructure.Persistence.Outbox;

public class OutboxProcessorBackgroundService : BackgroundService
{
    private const int MaxRetries = 5;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;
    private readonly IEventTypeMapper _typeMapper;

    public OutboxProcessorBackgroundService(
        IServiceScopeFactory scopeFactory,
        IEventTypeMapper typeMapper,
        ILogger<OutboxProcessorBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _typeMapper = typeMapper;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<UserServiceDbContext>();

                var dispatcher = scope.ServiceProvider
                    .GetRequiredService<IIntegrationEventDispatcher>();

                var strategy = dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await dbContext.Database
                        .BeginTransactionAsync(stoppingToken);
                    var messages = await dbContext.Set<OutboxMessage>()
                        .FromSqlRaw(@"
                            SELECT *
                            FROM ""OutboxMessages""
                            WHERE ""ProcessedOnUtc"" IS NULL
                              AND ""Retries"" < {0}
                            ORDER BY ""OccurredOnUtc""
                            FOR UPDATE SKIP LOCKED
                            LIMIT 20", MaxRetries)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        IIntegrationEvent? integrationEvent = null;

                        try
                        {
                            integrationEvent = Deserialize(message);
                            await dispatcher.DispatchAsync(
                                integrationEvent,
                                stoppingToken);

                            message.MarkAsProcessed();
                        }
                        catch (Exception ex)
                        {
                            message.MarkAsFailed($"message {ex.Message}, stack {ex.StackTrace}");
                            if (message.Retries >= MaxRetries)
                            {
                                try
                                {
                                    await dispatcher.DispatchToDeadLetterAsync(
                                        integrationEvent,
                                        ex,
                                        stoppingToken);
                                    _logger.LogError(
                                        "Message {MessageId} exceeded max retries. Marking as failed.",
                                        message.Id);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, "Message {MessageId} wasn't sent due to the exception.");
                                }
                            }
                            else
                            {
                                _logger.LogWarning(
                                    "Message {MessageId} failed, will retry. Attempt {Retries}/{MaxRetries}",
                                    message.Id, message.Retries, MaxRetries);
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                    await transaction.CommitAsync(stoppingToken);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processor failure");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private IIntegrationEvent Deserialize(OutboxMessage message)
    {
        var type = _typeMapper.GetType(message.Type);
        return (IIntegrationEvent)JsonSerializer.Deserialize(
            message.Payload,
            type)!;
    }
}