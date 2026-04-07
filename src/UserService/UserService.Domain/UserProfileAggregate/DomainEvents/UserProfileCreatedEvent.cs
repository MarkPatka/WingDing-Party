using UserService.Domain.Common.Abstract;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate.DomainEvents;

public sealed record UserProfileCreatedEvent(UserId UserId, DateTime CreatedAt) : IDomainEvent
{
    public DateTime OccurredOn => CreatedAt;
}