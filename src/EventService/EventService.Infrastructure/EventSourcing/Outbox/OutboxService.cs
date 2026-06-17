using EventService.Application.EventSourcing;
using EventService.Application.EventSourcing.Outbox;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Persistence.Outbox;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Outbox;

internal sealed class OutboxService : IOutboxService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly EventServiceDbContext _context;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(
        EventServiceDbContext context,
        ILogger<OutboxService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync<TEvent>(
        TEvent integrationEvent, CancellationToken cancellationToken = default) 
        where TEvent : IntegrationEvent
    {
        var content = JsonSerializer.Serialize(
            integrationEvent,
            integrationEvent.GetType(),
            JsonOptions);

        var message = new OutboxMessage(
            id: integrationEvent.Id,
            type: integrationEvent.EventType,
            content: content,
            occuredOnUtc: integrationEvent.OccurredOnUtc);

        await _context.OutboxMessages.AddAsync(message, cancellationToken);

        _logger.LogDebug(
            "Outbox message added: Type={Type}, Id={Id}",
            message.Type, message.Id);
    }
}
