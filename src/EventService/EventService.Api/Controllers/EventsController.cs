using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;
using EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;
using EventService.Application.EventManagement.Command.UpdateEventCommand;
using EventService.Application.EventManagement.Command.DeleteEventCommand;
using EventService.Application.EventManagement.Command.RegisterParticipant;
using EventService.Application.EventManagement.Queries.GetEventByIdQuery;
using EventService.Application.EventManagement.Queries.GetTopRatedEventsByStartDateWithLimitQuery;
using EventService.Application.EventManagement.Command.CreateEventTypeCommand;
using EventService.Application.EventManagement.Queries.GetAllEventTypesQuery;
using EventService.Application.EventManagement.Queries.GetEventParticipantsQuery;
using EventService.Application.EventManagement.Common;
using EventService.Contracts.Events.Requests;
using EventService.Contracts.Events.Responses;
using Microsoft.AspNetCore.Authorization;

namespace EventService.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]/[action]")]
public class EventsController(ISender sender, IMapper mapper) : ControllerBase
{
    [HttpPost(Name = nameof(CreateEventType))]
    public async Task<IActionResult> CreateEventType([FromBody] CreateEventTypeRequest request)
    {
        CreateEventTypeCommand command = mapper.Map<CreateEventTypeCommand>(request);
        CreateEventTypeResult result = await sender.Send(command);
        CreateEventTypeResponse response = mapper.Map<CreateEventTypeResponse>(result);
        return Ok(response);
    }

    [HttpGet(Name = nameof(GetAllEventTypes))]
    public async Task<IActionResult> GetAllEventTypes([FromQuery] GetAllEventTypesRequest request)
    {
        GetAllEventTypesQuery query = mapper.Map<GetAllEventTypesQuery>(request);
        GetAllEventTypesResult result = await sender.Send(query);
        GetAllEventTypesResponse response = mapper.Map<GetAllEventTypesResponse>(result);
        return Ok(response);
    }

    [HttpPost(Name = nameof(CreateEvent))]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
    {
        CreateEventCommand command = mapper.Map<CreateEventCommand>(request);
        CreateEventResult result = await sender.Send(command);
        CreateEventResponse response = mapper.Map<CreateEventResponse>(result);
        return Created(string.Empty, response);
    }

    [HttpPut("{eventId}", Name = nameof(UpdateEvent))]
    public async Task<IActionResult> UpdateEvent([FromRoute] Guid eventId, [FromBody] UpdateEventRequest request)
    {
        if (eventId != request.EventId)
            return BadRequest("Event ID in URL does not match ID in request body.");

        UpdateEventCommand command = mapper.Map<UpdateEventCommand>(request);
        UpdateEventResult result = await sender.Send(command);
        UpdateEventResponse response = mapper.Map<UpdateEventResponse>(result);
        return Ok(response);
    }

    [HttpDelete("{eventId}", Name = nameof(DeleteEvent))]
    public async Task<IActionResult> DeleteEvent([FromRoute] Guid eventId)
    {
        DeleteEventCommand command = mapper.Map<DeleteEventCommand>(new DeleteEventRequest(eventId));
        await sender.Send(command);
        return NoContent();
    }

    [HttpPost(Name = nameof(RegisterParticipantOnEvent))]
    public async Task<IActionResult> RegisterParticipantOnEvent([FromBody] RegisterParticipantRequest request)
    {
        RegisterParticipantCommand command = mapper.Map<RegisterParticipantCommand>(request);
        RegisterParticipantResult result = await sender.Send(command);
        RegisterParticipantResponse response = mapper.Map<RegisterParticipantResponse>(result);
        return Created(string.Empty, response);
    }

    [HttpGet("{userId}", Name = nameof(GetAllUserEvents))]
    public async Task<IActionResult> GetAllUserEvents(
        [FromRoute] Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        GetAllUserEventsQuery query = mapper.Map<GetAllUserEventsQuery>(
            new GetAllUserEventsRequest(userId, pageNumber, pageSize));
        GetAllUserEventsResult result = await sender.Send(query);
        GetAllUserEventsResponse response = mapper.Map<GetAllUserEventsResponse>(result);
        return Ok(response);
    }

    [HttpGet("{eventId}", Name = nameof(GetEventById))]
    public async Task<IActionResult> GetEventById([FromRoute] Guid eventId)
    {
        GetEventByIdQuery query = mapper.Map<GetEventByIdQuery>(new GetEventByIdRequest(eventId));
        GetEventByIdResult result = await sender.Send(query);
        GetEventByIdResponse response = mapper.Map<GetEventByIdResponse>(result);
        return Ok(response);
    }

    [HttpGet("{startDate}", Name = nameof(GetTopRatedEventsByStartDateWithLimit))]
    public async Task<IActionResult> GetTopRatedEventsByStartDateWithLimit(
        [FromRoute] DateTime startDate,
        [FromQuery] int limit = 10)
    {
        GetTopRatedEventsByStartDateWithLimitQuery query = mapper.Map<GetTopRatedEventsByStartDateWithLimitQuery>(
            new GetTopRatedEventsByStartDateWithLimitRequest(startDate, limit));
        GetTopRatedEventsByStartDateWithLimitResult result = await sender.Send(query);
        GetTopRatedEventsByStartDateWithLimitResponse response = mapper.Map<GetTopRatedEventsByStartDateWithLimitResponse>(result);
        return Ok(response);
    }

    [HttpGet("{text}", Name = nameof(GetEventsByTextAndFilters))]
    public async Task<IActionResult> GetEventsByTextAndFilters(
        [FromRoute] string text,
        [FromQuery] string? city,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        GetEventsByTextAndFiltersQuery query = mapper.Map<GetEventsByTextAndFiltersQuery>(
            new GetEventsByTextAndFiltersRequest(text, city, dateFrom, dateTo, pageNumber, pageSize));
        GetEventsByTextAndFiltersResult result = await sender.Send(query);
        GetEventsByTextAndFiltersResponse response = mapper.Map<GetEventsByTextAndFiltersResponse>(result);
        return Ok(response);
    }

    [HttpGet("{eventId}", Name = nameof(GetEventParticipants))]
    public async Task<IActionResult> GetEventParticipants([FromRoute] Guid eventId)
    {
        GetEventParticipantsQuery query = mapper.Map<GetEventParticipantsQuery>(
            new GetEventParticipantsRequest(eventId));
        GetEventParticipantsResult result = await sender.Send(query);
        GetEventParticipantsResponse response = mapper.Map<GetEventParticipantsResponse>(result);
        return Ok(response);
    }
}
