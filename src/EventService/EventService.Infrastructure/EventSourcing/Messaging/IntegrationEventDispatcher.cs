using EventService.Application.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public sealed class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IIntegrationEventTypeRegistry _registry;
    private readonly ILogger<IntegrationEventDispatcher> _logger;

    public IntegrationEventDispatcher(
        IServiceScopeFactory scopeFactory,
        IIntegrationEventTypeRegistry registry,
        ILogger<IntegrationEventDispatcher> logger)
    {
        _scopeFactory = scopeFactory;
        _registry = registry;
        _logger = logger;
    }

    public async Task DispatchAsync(
        string eventType, string payload, CancellationToken cancellationToken)
    {
        if (!_registry.TryResolve(eventType, out var concreteType)
            || concreteType is null)
        {
            _logger.LogWarning(
                "No registered type for EventType '{EventType}'. Skipping",
                eventType);
            return;
        }

        var integrationEvent = (IntegrationEvent?)JsonSerializer.Deserialize(
            payload, concreteType, JsonOptions);

        if (integrationEvent is null)
            throw new InvalidOperationException(
                $"Deserialization returned null for {eventType}");

        var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(concreteType);

        await using var scope = _scopeFactory.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetService(handlerType);

        if (handler is null)
        {
            _logger.LogWarning(
                "No handler registered for {EventType}. Skipping.", eventType);
            return;
        }

        var method = handlerType.GetMethod(
            nameof(IIntegrationEventHandler<IntegrationEvent>.Handle))!;
        await (Task)method.Invoke(handler, [integrationEvent, cancellationToken])!;
    }
}
