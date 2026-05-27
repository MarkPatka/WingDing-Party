using UserService.Application.IntegrationEvents.UserProfiles;
using UserService.Domain.UserProfileAggregate.DomainEvents;

namespace UserService.Application.IntegrationEvents.Mapping;

public sealed class IntegrationEventTypeResolver : IIntegrationEventTypeResolver
{
    private static readonly IReadOnlyDictionary<Type, Type> _mapping =
        new Dictionary<Type, Type>
        {
            [typeof(UserProfileCreatedEvent)] = typeof(UserProfileCreatedIntegrationEvent),
            [typeof(UserProfileUpdatedEvent)] = typeof(UserProfileUpdatedIntegrationEvent),
        };

    public Type? Resolve(Type domainEventType)
        => _mapping.TryGetValue(domainEventType, out var type) ? type : null;
}