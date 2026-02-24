using ClubService.Application.Common.Attributes;

namespace ClubService.Application.IntegrationEvents.Clubs;

[Aggregate("club")]
public class ClubCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}