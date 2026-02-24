using ClubService.Application.IntegrationEvents.Clubs;

namespace ClubService.Infrastructure.Messaging;

public class EventTypeMapper : IEventTypeMapper
{
    private static readonly Dictionary<string, Type> _types = new()
    {
        {
            nameof(ClubCreatedIntegrationEvent),
            typeof(ClubCreatedIntegrationEvent)
        }
    };

    public Type GetType(string typeName)
    {
        if (!_types.TryGetValue(typeName, out var type))
            throw new InvalidOperationException(
                $"Unknown integration event type {typeName}");

        return type;
    }

    public string GetName(Type type)
    {
        return type.Name;
    }
}