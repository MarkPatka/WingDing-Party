using ClubService.Domain.ClubAggregate.ValueObjects;
using ClubService.Domain.Common.Abstract;

namespace ClubService.Domain.ClubAggregate.DomainEvents;

public sealed record UserProfileCreatedDomainEvent(UserId UserId, DateTime CreatedAt) : IDomainEvent
{
    public DateTime OccurredOn => CreatedAt;
}
