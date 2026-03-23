using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;
using EventService.Contracts.Events;
using EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;
using EventService.Application.EventManagement.Command.UpdateEventCommand;
using EventService.Application.EventManagement.Command.DeleteEventCommand;
using EventService.Application.EventManagement.Command.RegisterParticipant;
using EventService.Application.EventManagement.Queries.GetEventByIdQuery;
using EventService.Application.EventManagement.Queries.GetTopRatedEventsByStartDateWithLimitQuery;

namespace EventService.Api.Controllers;

[Route("[controller]/[action]")]
public class EventsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public EventsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost(Name = nameof(CreateEvent))]
    public async Task<IActionResult> CreateEvent(CreateEventRequest request)
    {
        var command = _mapper.Map<CreateEventCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<CreateEventResponse>(result);

        return Ok(response);
    }

    [HttpPut("{EventId}", Name = nameof(UpdateEvent))]
    public async Task<IActionResult> UpdateEvent(UpdateEventRequest request)
    {
        var command = _mapper.Map<UpdateEventCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<UpdateEventResponse>(result);

        return Ok(response);
    }

    [HttpDelete("{EventId}", Name = nameof(DeleteEvent))]
    public async Task<IActionResult> DeleteEvent(DeleteEventRequest request)
    {
        var command = _mapper.Map<DeleteEventCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<DeleteEventResponse>(result);

        return Ok(response);
    }

    [HttpPost(Name = nameof(RegisterParticipantOnEvent))]
    public async Task<IActionResult> RegisterParticipantOnEvent(RegisterParticipantRequest request)
    {
        var command = _mapper.Map<RegisterParticipantCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<RegisterParticipantResponse>(result);

        return Ok(response);
    }


    [HttpGet("{UserId}", Name = nameof(GetAllUserEvents))]
    public async Task<IActionResult> GetAllUserEvents(GetAllUserEventsRequest request)
    {
        var query = _mapper.Map<GetAllUserEventsQuery>(request);

        var result = await _sender.Send(query);

        var response = _mapper.Map<GetAllUserEventsResponse>(result);

        return Ok(response);
    }

    [HttpGet("{EventId}", Name = nameof(GetEventById))]
    public async Task<IActionResult> GetEventById(GetEventByIdRequest request)
    {
        var query = _mapper.Map<GetEventByIdQuery>(request);

        var result = await _sender.Send(query);

        var response = _mapper.Map<GetEventByIdResponse>(result);
        
        return Ok(response);
    }

    [HttpGet("{StartDate}", Name = nameof(GetTopRatedEventsByStartDateWithLimit))]
    public async Task<IActionResult> GetTopRatedEventsByStartDateWithLimit(
        GetTopRatedEventsByStartDateWithLimitRequest request)
    {
        var query = _mapper.Map<GetTopRatedEventsByStartDateWithLimitQuery>(request);

        var result = await _sender.Send(query);

        var response = _mapper.Map<GetTopRatedEventsByStartDateWithLimitResponse>(result);

        return Ok(response);
    }

    [HttpGet("{Text}", Name = nameof(GetEventsByTextAndFilters))]
    public async Task<IActionResult> GetEventsByTextAndFilters(
        GetEventsByTextAndFiltersRequest request)
    {
        var query = _mapper.Map<GetEventsByTextAndFiltersQuery>(request);

        var result = await _sender.Send(query);

        var response = _mapper.Map<GetEventsByTextAndFiltersResponse>(result);

        return Ok(response);
    }
}
