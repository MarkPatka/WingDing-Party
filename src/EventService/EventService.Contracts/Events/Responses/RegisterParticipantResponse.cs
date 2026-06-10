namespace EventService.Contracts.Events.Responses;

public sealed record RegisterParticipantResponse(Guid ParticipantId, DateTime RegisteredAt);
