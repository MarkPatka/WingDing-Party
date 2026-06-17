using EventService.Application.EventSourcing.IntegrationEvents.Outgoing;

namespace EventService.Infrastructure.EventSourcing.Outbox;

internal static class OutgoingTypes
{
    private static readonly IReadOnlyDictionary<string, Type> _types =
        new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(EventCreatedIntegrationEvent)]          = typeof(EventCreatedIntegrationEvent),
            [nameof(EventUpdatedIntegrationEvent)]          = typeof(EventUpdatedIntegrationEvent),
            [nameof(EventDeletedIntegrationEvent)]          = typeof(EventDeletedIntegrationEvent),
            [nameof(EventPublishedIntegrationEvent)]        = typeof(EventPublishedIntegrationEvent),
            [nameof(EventCancelledIntegrationEvent)]        = typeof(EventCancelledIntegrationEvent),
            [nameof(ParticipantRegisteredIntegrationEvent)] = typeof(ParticipantRegisteredIntegrationEvent)
        };

    public static Type? Resolve(string typeName)
        => _types.TryGetValue(typeName, out var type) ? type : null;
}
