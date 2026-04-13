using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Models.Request;
using UserService.Application.Services;
using UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;
using UserService.Application.UserProfileManagement.Command.UpdateUserProfileCommand;
using UserService.Application.UserProfileManagement.Command.UpdateUserProfileInterestsCommand;
using UserService.Application.UserProfileManagement.Common;
using UserService.Application.UserProfileManagement.Queries.GetUserProfileInterestsQuery;
using UserService.Application.UserProfileManagement.Queries.GetUserProfileQuery;
using UserService.Contracts.UserProfiles;

namespace UserService.Api.Controllers;

[Route("profiles")]
public class UserProfileController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly IFileStorage _fileStorage;


    public UserProfileController(IFileStorage fileStorage, ISender sender, IMapper mapper)
    {
        _fileStorage = fileStorage;
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserProfile(GetUserProfileRequest request)
    {
        var query = _mapper.Map<GetUserProfileQuery>(request);

        var result = await _sender.Send(query);

        var response = _mapper.Map<GetUserProfileResponse>(result);

        return Ok(response);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateUserProfile([FromForm] CreateUserProfileForm form)
    {
        Uri? avatarUri = null;
        if (form.Avatar != null)
        {
            using var stream = form.Avatar.OpenReadStream();
            form.AvatarUri = await _fileStorage.SaveAsync(
                stream,
                form.Avatar.FileName,
                "",
                form.Avatar.ContentType,
                HttpContext.RequestAborted);
        }

        var request = form.Adapt<CreateUserProfileRequest>();

        var command = _mapper.Map<CreateUserProfileCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<CreateUserProfileResponse>(result);

        return Ok(response);
    }


    [HttpPut]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request)
    {
        var command = _mapper.Map<UpdateUserProfileCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<UpdateUserProfileResult>(result);

        return Ok(response);
    }

    [HttpGet("interests")]
    public async Task<IActionResult> GetUserProfileInterests(GetUserProfileInterestsRequest request)
    {
        var query = _mapper.Map<GetUserProfileInterestsQuery>(request);

        var result = await _sender.Send(query);

        var response = _mapper.Map<GetUserProfileInterestsResult>(result);

        return Ok(response);
    }

    [HttpPut("interests")]
    public async Task<IActionResult> UpdateUserProfileInterests([FromBody] UpdateUserProfileInterestsRequest request)
    {
        var command = _mapper.Map<UpdateUserProfileInterestsCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<UpdateUserProfileInterestsResult>(result);

        return Ok(response);
    }
}