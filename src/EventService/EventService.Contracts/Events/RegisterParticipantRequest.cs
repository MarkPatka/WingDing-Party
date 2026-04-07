namespace EventService.Contracts.Events;

public sealed record RegisterParticipantRequest(
    Guid EventId,
    Guid UserId,
    string UserName);
