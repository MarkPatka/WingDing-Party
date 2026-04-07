using ClubService.Application.IntegrationEvents;
using ClubService.Application.IntegrationEvents.Clubs;
using ClubService.Domain.ClubAggregate.DomainEvents;
using ClubService.Domain.Common.Abstract;
using Mapster;

namespace ClubService.Infrastructure.Messaging.Mapping;

public class ClubIntegrationMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IDomainEvent, IIntegrationEvent>()
            .Include<ClubCreatedDomainEvent, ClubCreatedIntegrationEvent>()
            .Include<ClubDeletedDomainEvent, ClubDeletedIntegrationEvent>();

        config.NewConfig<ClubCreatedDomainEvent, ClubCreatedIntegrationEvent>()
            .Map(dest => dest.Id, src => src.ClubId.Value)
            .Map(dest => dest.OccurredOnUtc, src => src.CreatedAt.ToUniversalTime());

        config.NewConfig<ClubDeletedDomainEvent, ClubDeletedIntegrationEvent>()
            .Map(dest => dest.Id, src => src.ClubId.Value)
            .Map(dest => dest.OccurredOnUtc, src => src.CreatedAt.ToUniversalTime());
    }
}