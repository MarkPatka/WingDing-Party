using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;
using EventService.Contracts.Events;

namespace EventService.Api.Controllers;

public class EventsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;


    public EventsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost("create")]
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


    [HttpGet("get")]
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

}
