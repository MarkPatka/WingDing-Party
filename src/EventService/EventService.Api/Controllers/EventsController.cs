using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;
using EventService.Contracts.Events;
using EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;
using EventService.Application.EventManagement.Command.UpdateEventCommand;
using EventService.Application.EventManagement.Command.DeleteEventCommand;

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
        // request -> map to command
        var command = _mapper.Map<CreateEventCommand>(request);

        // send command to request handler
        var result = await _sender.Send(command);

        // map the result model to response model 
        var response = _mapper.Map<CreateEventResponse>(result);

        // get the handler response 
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


    [HttpGet("{UserId}", Name = "GetAllUserEvents")]
    public async Task<IActionResult> GetAllUserEvents(GetAllUserEventsRequest request)
    {
        // request -> map to command
        var query = _mapper.Map<GetAllUserEventsQuery>(request);

        // send command to request handler
        var result = await _sender.Send(query);

        // map the result model to response model 
        var response = _mapper.Map<GetAllUserEventsResponse>(result);

        // get the handler response 
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
