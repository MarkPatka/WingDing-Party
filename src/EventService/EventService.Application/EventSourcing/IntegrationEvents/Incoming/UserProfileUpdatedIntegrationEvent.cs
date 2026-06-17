namespace EventService.Application.EventSourcing.IntegrationEvents.Incoming;

public sealed record UserProfileUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;

    public UserProfileUpdatedIntegrationEvent()
    {
        EventType = nameof(UserProfileUpdatedIntegrationEvent);
    }
}
