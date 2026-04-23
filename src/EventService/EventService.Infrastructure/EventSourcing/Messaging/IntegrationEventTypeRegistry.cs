using EventService.Application.EventSourcing;
using EventService.Application.EventSourcing.IntegrationEvents.Incoming;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public sealed class IntegrationEventTypeRegistry : IIntegrationEventTypeRegistry
{
    private static readonly IReadOnlyDictionary<string, Type> _types =
        new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(UserProfileUpdatedIntegrationEvent)] = typeof(UserProfileUpdatedIntegrationEvent)
        };
    public string GetName<TEvent>() where TEvent : IntegrationEvent
        => typeof(TEvent).Name;

    public bool TryResolve(string eventType, out Type? type)
        => _types.TryGetValue(eventType, out type);
}
