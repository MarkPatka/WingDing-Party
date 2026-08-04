using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Models.Request;
using UserService.Application.AvatarManagement.Commands.CreateAvatarCommand;
using UserService.Application.AvatarManagement.Commands.DeleteAvatarCommand;
using UserService.Application.AvatarManagement.Commands.UpdateAvatarCommand;
using UserService.Contracts.Avatars;
using WingDing.SharedKernel.Auth;
using WingDing.SharedKernel.Auth.Authorization;

namespace UserService.Api.Controllers;

[Route("avatars")]
public class AvatarController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;


    public AvatarController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.UsersUpdate)]
    public async Task<IActionResult> CreateAvatarProfile([FromForm] CreateAvatarForm form)
    {
        using var stream = form.Avatar != null ? form.Avatar.OpenReadStream() : null;
        form.AvatarStream = stream;
        form.FileName = form.Avatar != null ? form.Avatar.FileName : string.Empty;
        form.ContentType = form.Avatar != null ? form.Avatar.ContentType : string.Empty;

        var request = _mapper.Map<CreateAvatarRequest>(form);

        var command = _mapper.Map<CreateAvatarCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<CreateAvatarResponse>(result);

        return Ok(response);
    }

    [HttpPut]
    [HasPermission(Permissions.UsersUpdate)]
    public async Task<IActionResult> UpdateAvatarProfile([FromBody] UpdateAvatarRequest request)
    {
        var command = _mapper.Map<UpdateAvatarCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<UpdateAvatarResponse>(result);

        return Ok(response);
    }

    [HttpDelete]
    [HasPermission(Permissions.UsersUpdate)]
    public async Task<IActionResult> DeleteAvatarProfile([FromBody] DeleteAvatarRequest request)
    {
        var command = _mapper.Map<DeleteAvatarCommand>(request);

        var result = await _sender.Send(command);

        var response = _mapper.Map<DeleteAvatarResponse>(result);

        return Ok(response);
    }
}