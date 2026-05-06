using UserService.Application.IntegrationEvents.UserProfiles;
using UserService.Domain.UserProfileAggregate.DomainEvents;

namespace UserService.Application.IntegrationEvents.Mapping;

public sealed class UserProfileUpdatedEventMapper
    : IntegrationEventMapper<UserProfileUpdatedEvent, UserProfileUpdatedIntegrationEvent>
{
    protected override IIntegrationEvent MapCore(
        UserProfileUpdatedEvent domainEvent)
    {
        return new UserProfileUpdatedIntegrationEvent
        {
            UserId = domainEvent.UserId.Value,
            DisplayName = domainEvent.DisplayName,
            OccurredOnUtc = domainEvent.UpdatedAt
        };
    }
}
