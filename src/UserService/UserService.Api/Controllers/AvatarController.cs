using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Models.Request;
using UserService.Application.AvatarManagement.Commands.CreateAvatarCommand;
using UserService.Contracts.Avatars;
using UserService.Contracts.UserProfiles;

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
}