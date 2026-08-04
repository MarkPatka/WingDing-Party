using AuthService.Application.UserManagement.Command.AssignRole;
using AuthService.Contracts.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WingDing.SharedKernel.Auth;
using WingDing.SharedKernel.Auth.Authorization;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/admin/")]
public class AdminController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public AdminController(ISender sender, IMapper mapper)
        => (_sender, _mapper) = (sender, mapper);

    [HttpPost("user/{id:guid}/assign-role")]
    [HasPermission(Permissions.AdminPanel)]
    public async Task<IActionResult> Assign([FromRoute] Guid id, [FromBody] AssignRoleRequest request)
    {
        var command = _mapper.Map<AssignRoleCommand>(request with { Id = id });
        var result = await _sender.Send(command);
        var response = _mapper.Map<AssignRoleResponse>(result);
        return Ok(response);
    }
}