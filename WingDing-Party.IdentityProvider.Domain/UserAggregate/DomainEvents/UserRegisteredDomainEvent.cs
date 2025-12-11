using WingDing_Party.IdentityProvider.Domain.Common.Abstract;
using WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.DomainEvents;

public sealed record UserRegisteredDomainEvent(UserId userId, Email email) 
    : IDomainEvent
{
    public UserId UserId { get; } = userId;
    public Email Email { get; } = email;
}
