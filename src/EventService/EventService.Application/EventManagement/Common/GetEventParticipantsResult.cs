using EventService.Contracts.DTO;

namespace EventService.Application.EventManagement.Common;

public record GetEventParticipantsResult(IEnumerable<ParticipantDto> Participants);
