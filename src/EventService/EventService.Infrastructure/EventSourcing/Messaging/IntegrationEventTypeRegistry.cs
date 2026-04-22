using EventService.Application.EventSourcing;
using EventService.Infrastructure.EventSourcing.EventContracts;

namespace EventService.Infrastructure.EventSourcing.Messaging;

public sealed class IntegrationEventTypeRegistry : IIntegrationEventTypeRegistry
{
    private static readonly IReadOnlyDictionary<string, Type> _types =
        new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(EventCreatedIntegrationEvent)] = typeof(EventCreatedIntegrationEvent),
            [nameof(EventUpdatedIntegrationEvent)] = typeof(EventUpdatedIntegrationEvent),
            [nameof(EventDeletedIntegrationEvent)] = typeof(EventDeletedIntegrationEvent),
            [nameof(EventPublishedIntegrationEvent)] = typeof(EventPublishedIntegrationEvent),
            [nameof(EventCancelledIntegrationEvent)] = typeof(EventCancelledIntegrationEvent),
            [nameof(ParticipantRegisteredIntegrationEvent)] = typeof(ParticipantRegisteredIntegrationEvent)
        };
    public string GetName<TEvent>() where TEvent : IntegrationEvent
        => typeof(TEvent).Name;

    public bool TryResolve(string eventType, out Type? type)
        => _types.TryGetValue(eventType, out type);
}
