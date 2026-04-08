using ClubService.Application.ClubManagement.Command.CreateClubCommand;
using ClubService.Application.ClubManagement.Command.DeleteClubCommand;
using ClubService.Application.ClubManagement.Command.JoinClubCommand;
using ClubService.Application.ClubManagement.Command.LeaveClubCommand;
using ClubService.Application.ClubManagement.Command.UpdateClubCommand;
using ClubService.Application.ClubManagement.Queries.GetClubMembersQuery;
using ClubService.Application.ClubManagement.Queries.GetClubQuery;
using ClubService.Application.ClubManagement.Queries.GetClubsByUserQuery;
using ClubService.Application.ClubManagement.Queries.SearchClubsQuery;
using ClubService.Contracts.Clubs;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.Api.Controllers;

[Route("club")]
public class ClubController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;


    public ClubController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetClub(GetClubRequest request)
    {
        var query = _mapper.Map<GetClubQuery>(request);
        var result = await _sender.Send(query);
        var response = _mapper.Map<GetClubResponse>(result);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClub([FromBody] CreateClubRequest request)
    {
        var command = _mapper.Map<CreateClubCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<CreateClubResponse>(result);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateClub([FromBody] UpdateClubRequest request)
    {
        var command = _mapper.Map<UpdateClubCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<UpdateClubResponse>(result);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteClub(DeleteClubRequest request)
    {
        var command = _mapper.Map<DeleteClubCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<DeleteClubResponse>(result);
        return Ok(response);
    }

    [HttpGet("participant")]
    public async Task<IActionResult> GetClubsByParticipant(GetClubsByUserRequest request)
    {
        var query = _mapper.Map<GetClubsByUserQuery>(request);
        var result = await _sender.Send(query);
        var response = _mapper.Map<IEnumerable<GetClubsByUserResponse>>(result);
        return Ok(response);
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetClubMembers(GetClubMembersRequest request)
    {
        var query = _mapper.Map<GetClubMembersQuery>(request);
        var result = await _sender.Send(query);
        var response = _mapper.Map<IEnumerable<GetClubMembersResponse>>(result);
        return Ok(response);
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinToClub([FromBody] JoinToClubRequest request)
    {
        var command = _mapper.Map<JoinToClubCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<JoinToClubResponse>(result);
        return Ok(response);
    }

    [HttpPost("leave")]
    public async Task<IActionResult> LeaveClub([FromBody] LeaveClubRequest request)
    {
        var command = _mapper.Map<LeaveClubCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<LeaveClubResponse>(result);
        return Ok(response);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchClubs([FromBody] SearchClubsRequest request)
    {
        var query = _mapper.Map<SearchClubsQuery>(request);
        var result = await _sender.Send(query);
        var response = _mapper.Map<IEnumerable<SearchClubsResponse>>(result);
        return Ok(response);
    }
}