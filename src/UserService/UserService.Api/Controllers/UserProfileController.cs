using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;
using UserService.Application.UserProfileManagement.Command.UpdateUserProfileCommand;
using UserService.Application.UserProfileManagement.Command.UpdateUserProfileInterestsCommand;
using UserService.Application.UserProfileManagement.Common;
using UserService.Application.UserProfileManagement.Queries.GetUserProfileInterestsQuery;
using UserService.Application.UserProfileManagement.Queries.GetUserProfileQuery;
using UserService.Contracts.UserProfiles;
using WingDing.Auth.Shared;
using WingDing.Auth.Shared.Authorization;

namespace UserService.Api.Controllers;

[Route("profiles")]
public class UserProfileController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;


    public UserProfileController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetUserProfile(GetUserProfileRequest request)
    {
        var query = _mapper.Map<GetUserProfileQuery>(request);

        var result = await _sender.Send(query);

        var response = _mapper.Map<GetUserProfileResponse>(result);

        return Ok(response);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.UsersUpdate)]
    public async Task<IActionResult> CreateUserProfile([FromForm] CreateUserProfileRequest request)
    {
        var command = _mapper.Map<CreateUserProfileCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<CreateUserProfileResponse>(result);

        return Ok(response);
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.UsersUpdate)]
    public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserProfileCommand request)
    {
        var command = _mapper.Map<UpdateUserProfileCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<UpdateUserProfileResponse>(result);

        return Ok(response);
    }

    [HttpGet("interests")]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetUserProfileInterests(GetUserProfileInterestsRequest request)
    {
        var query = _mapper.Map<GetUserProfileInterestsQuery>(request);

        var result = await _sender.Send(query);

        var response = _mapper.Map<GetUserProfileInterestsResult>(result);

        return Ok(response);
    }

    [HttpPut("interests")]
    [HasPermission(Permissions.UsersUpdate)]
    public async Task<IActionResult> UpdateUserProfileInterests([FromBody] UpdateUserProfileInterestsRequest request)
    {
        var command = _mapper.Map<UpdateUserProfileInterestsCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<UpdateUserProfileInterestsResult>(result);

        return Ok(response);
    }
}