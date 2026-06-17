using UserService.Application.Common.Attributes;

namespace UserService.Application.IntegrationEvents.UserProfiles;

[Aggregate("userprofile")]
public class UserProfileUpdatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = nameof(UserProfileUpdatedIntegrationEvent);

    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;

}
