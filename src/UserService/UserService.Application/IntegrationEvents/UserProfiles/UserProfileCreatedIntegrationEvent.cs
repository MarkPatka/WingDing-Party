using UserService.Application.Common.Attributes;

namespace UserService.Application.IntegrationEvents.UserProfiles;

[Aggregate("userprofile")]
public class UserProfileCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = nameof(UserProfileCreatedIntegrationEvent);
}