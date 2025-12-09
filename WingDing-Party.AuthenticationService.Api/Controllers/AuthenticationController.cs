using MediatR;
using Microsoft.AspNetCore.Mvc;
using WingDing_Party.AuthenticationService.Application.Authentication.Commands.Register;
using WingDing_Party.AuthenticationService.Application.Authentication.Queries.Login;
using WingDing_Party.AuthenticationService.Contracts.Authentication;

namespace WingDing_Party.AuthenticationService.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _sender;

    public AuthenticationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);
        
        var responce = await _sender.Send(command);

        return Ok(responce);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);
        var response = _sender.Send(query);

        return Ok(response);
    }

}
