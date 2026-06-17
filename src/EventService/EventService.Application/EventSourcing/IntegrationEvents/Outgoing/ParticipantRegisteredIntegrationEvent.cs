namespace EventService.Application.EventSourcing.IntegrationEvents.Outgoing;

public sealed record ParticipantRegisteredIntegrationEvent : IntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid ParticipantId { get; init; }
    public DateTime RegisteredAt { get; init; }

    public ParticipantRegisteredIntegrationEvent()
    {
        EventType = nameof(ParticipantRegisteredIntegrationEvent);
    }
}
