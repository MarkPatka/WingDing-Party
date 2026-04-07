namespace EventService.Contracts.Events;

public sealed record RegisterParticipantResponse(Guid ParticipantId, DateTime RegisteredAt);
