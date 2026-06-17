using Mapster;
using UserService.Application.IntegrationEvents.UserProfiles;
using UserService.Domain.UserProfileAggregate.DomainEvents;

namespace UserService.Application.IntegrationEvents.Mapping;

public class UserIntegrationMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserProfileCreatedEvent, UserProfileCreatedIntegrationEvent>()
            .Map(dest => dest.OccurredOnUtc, src => src.OccurredOn);

        config.NewConfig<UserProfileUpdatedEvent, UserProfileUpdatedIntegrationEvent>()
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.DisplayName, src => src.DisplayName)
            .Map(dest => dest.OccurredOnUtc, src => src.UpdatedAt);
    }
}
