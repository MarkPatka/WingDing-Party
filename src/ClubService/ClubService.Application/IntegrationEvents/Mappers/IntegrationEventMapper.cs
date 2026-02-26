using ClubService.Application.IntegrationEvents.Clubs;
using ClubService.Application.IntegrationEvents.UserProfiles;
using ClubService.Domain.ClubAggregate.DomainEvents;
using ClubService.Domain.Common.Abstract;

namespace ClubService.Application.IntegrationEvents.Mappers;

public static class IntegrationEventMapper
{
    public static IIntegrationEvent? Map(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            ClubCreatedDomainEvent e => new ClubCreatedIntegrationEvent
            {
                Id = e.ClubId.Value,
            },
            ClubDeletedDomainEvent e => new ClubDeletedIntegrationEvent()
            {
                Id = e.ClubId.Value,
            },
            UserProfileCreatedDomainEvent e => new UserProfileCreatedIntegrationEvent()
            {
                Id = e.UserId.Value,
            },
            _ => null
        };
    }
}