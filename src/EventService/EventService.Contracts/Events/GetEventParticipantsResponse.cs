using EventService.Contracts.DTO;

namespace EventService.Contracts.Events;

public sealed record GetEventParticipantsResponse(
    IEnumerable<ParticipantDto> Participants);
