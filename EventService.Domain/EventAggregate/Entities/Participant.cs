using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.Enumerations;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.Entities;

public sealed class Participant : Entity<ParticipantId>
{
    public EventId EventId { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Registered;

    private Participant() { }

    public Participant(ParticipantId id) : base(id) { }
}
