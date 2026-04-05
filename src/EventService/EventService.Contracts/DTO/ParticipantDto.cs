namespace EventService.Contracts.DTO;

public sealed record ParticipantDto(
    string ParticipantId,
    string UserId,
    string UserName,
    DateTime RegisteredAt,
    string Status);
