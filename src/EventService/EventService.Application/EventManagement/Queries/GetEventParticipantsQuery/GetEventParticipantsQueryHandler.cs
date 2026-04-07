using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Services;
using EventService.Contracts.DTO;
using EventService.Domain.EventAggregate.ValueObjects;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetEventParticipantsQuery;

public class GetEventParticipantsQueryHandler
    : IRequestHandler<GetEventParticipantsQuery, GetEventParticipantsResult>
{
    private readonly IEventService _eventService;

    public GetEventParticipantsQueryHandler(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<GetEventParticipantsResult> Handle(
        GetEventParticipantsQuery request, CancellationToken cancellationToken)
    {
        var @event = await _eventService.GetEventParticipantsAsync(
            EventId.Create(request.EventId), cancellationToken);

        if (@event is null)
            throw new EntityNotFoundException($"Event {request.EventId} not found");

        var participantDtos = @event.Participants.Select(p => new ParticipantDto(
            p.Id.Value.ToString(),
            p.UserId.Value.ToString(),
            p.UserName,
            p.RegisteredAt,
            p.Status.ToString())).ToList();

        return new GetEventParticipantsResult(participantDtos);
    }
}
