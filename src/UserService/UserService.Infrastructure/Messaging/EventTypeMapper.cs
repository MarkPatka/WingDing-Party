using UserService.Application.IntegrationEvents.UserProfiles;

namespace UserService.Infrastructure.Messaging;

public class EventTypeMapper : IEventTypeMapper
{
    private static readonly Dictionary<string, Type> _types = new()
    {
        {
            nameof(UserProfileCreatedIntegrationEvent),
            typeof(UserProfileCreatedIntegrationEvent)
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