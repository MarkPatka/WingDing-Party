using AuthService.Contracts.Constants;
using AuthService.Infrastructure.Authorization;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Register(/* RegisterUserRequest */)
    {
        // TODO: MediatR command -> creates user in Keycloak + local DB
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(/* LoginRequest */)
    {
        // TODO: MediatR command -> gets JWT from Keycloak token endpoint
        return Ok();
    }

    [HttpGet("me")]
    [HasPermission(Permissions.UsersRead)]  // <-- THIS IS THE PATTERN
    public async Task<IActionResult> GetMe()
    {
        // Only accessible if the user's roles include a permission "users:read"
        return Ok();
    }
}