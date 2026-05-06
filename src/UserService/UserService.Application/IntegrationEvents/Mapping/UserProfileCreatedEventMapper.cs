using UserService.Application.IntegrationEvents.UserProfiles;
using UserService.Domain.UserProfileAggregate.DomainEvents;

namespace UserService.Application.IntegrationEvents.Mapping;

public sealed class UserProfileCreatedEventMapper
    : IntegrationEventMapper<UserProfileCreatedEvent, UserProfileCreatedIntegrationEvent>
{
    protected override IIntegrationEvent MapCore(
        UserProfileCreatedEvent domainEvent)
    {
        return new UserProfileCreatedIntegrationEvent
        {
            OccurredOnUtc = domainEvent.OccurredOn
        };
    }
}
