using Mapster;
using UserService.Application.IntegrationEvents;
using UserService.Application.IntegrationEvents.UserProfiles;
using UserService.Domain.Common.Abstract;
using UserService.Domain.UserProfileAggregate.DomainEvents;

namespace ClubService.Infrastructure.Messaging.Mapping;

public class UserIntegrationMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IDomainEvent, IIntegrationEvent>()
            .Include<UserProfileCreatedEvent, UserProfileCreatedIntegrationEvent>()
            .Include<UserProfileUpdatedEvent, UserProfileUpdatedIntegrationEvent>();

        config.NewConfig<UserProfileUpdatedEvent, UserProfileUpdatedIntegrationEvent>()
            .Map(dest => dest.UserId,           src => src.UserId.Value)
            .Map(dest => dest.DisplayName,      src => src.DisplayName)
            .Map(dest => dest.OccurredOnUtc,    src => src.UpdatedAt);
    }
}