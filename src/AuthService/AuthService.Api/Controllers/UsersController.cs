using AuthService.Application.UserManagement.Command.RegisterUser;
using AuthService.Application.UserManagement.Queries.LoginUser;
using AuthService.Contracts.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WingDing.Auth.Shared;
using WingDing.Auth.Shared.Authorization;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public UsersController(ISender sender, IMapper mapper)
        => (_sender, _mapper) = (sender, mapper);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var command = _mapper.Map<RegisterUserCommand>(request);    
        var result = await _sender.Send(command);
        var response = _mapper.Map<RegisterUserResponse>(result);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var query = _mapper.Map<LoginUserQuery>(request);
        var result = await _sender.Send(query);
        
        if (result is null)
        {
            return Unauthorized();
        }   

        var response = _mapper.Map<LoginResponse>(result);
        return Ok(response);
    }

    [HttpGet("me")]
    [HasPermission(Permissions.UsersRead)]  // <-- THIS IS THE PATTERN
    public async Task<IActionResult> GetMe()
    {
        // Only accessible if the user's roles include a permission "users:read"
        return Ok();
    }
}