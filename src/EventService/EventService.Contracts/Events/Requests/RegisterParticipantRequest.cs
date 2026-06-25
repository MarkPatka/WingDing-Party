namespace EventService.Contracts.Events.Requests;

public sealed record RegisterParticipantRequest(
    Guid EventId,
    Guid UserId,
    string UserName);
