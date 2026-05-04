using UserService.Domain.Common.Abstract;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate.DomainEvents;

public sealed record UserProfileUpdatedEvent(
    UserId UserId,
    string DisplayName,
    DateTime UpdatedAt) : IDomainEvent
{
    public DateTime OccuredOn => UpdatedAt;
}
