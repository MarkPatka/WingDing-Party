using EventService.Contracts.DTO;

namespace EventService.Contracts.Events.Responses;

public sealed record GetEventParticipantsResponse(
    IEnumerable<ParticipantDto> Participants);
