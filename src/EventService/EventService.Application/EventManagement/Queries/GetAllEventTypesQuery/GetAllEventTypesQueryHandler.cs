using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Services;
using EventService.Contracts.DTO;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetAllEventTypesQuery;

public class GetAllEventTypesQueryHandler
    : IRequestHandler<GetAllEventTypesQuery, GetAllEventTypesResult>
{
    private readonly IEventTypeService _eventTypeService;

    public GetAllEventTypesQueryHandler(IEventTypeService eventTypeService)
    {
        _eventTypeService = eventTypeService;
    }

    public async Task<GetAllEventTypesResult> Handle(
        GetAllEventTypesQuery request, CancellationToken cancellationToken)
    {
        var eventTypes = await _eventTypeService.GetAllEventTypesAsync(
            request.PageNumber, request.PageSize, cancellationToken);

        if (!eventTypes.Any())
            throw new EntityNotFoundException("EventTypes not found");

        var eventTypeDtos = eventTypes.Select(et => new EventTypeDto(
            et.Id.Value.ToString(),
            et.Name,
            et.Description,
            et.Icon
            ));

        return new GetAllEventTypesResult(eventTypeDtos);
    }
}
