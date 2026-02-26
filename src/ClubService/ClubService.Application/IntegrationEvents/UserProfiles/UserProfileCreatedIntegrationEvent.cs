using ClubService.Application.Common.Attributes;

namespace ClubService.Application.IntegrationEvents.UserProfiles;

[Aggregate("userprofile")]
public class UserProfileCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}