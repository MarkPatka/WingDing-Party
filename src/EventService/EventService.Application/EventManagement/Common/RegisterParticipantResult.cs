namespace EventService.Application.EventManagement.Common;

public record RegisterParticipantResult(
    Guid ParticipantId,
    DateTime RegisteredAt);
