using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetEventParticipantsQuery;

public record GetEventParticipantsQuery(Guid EventId) : IRequest<GetEventParticipantsResult>;
