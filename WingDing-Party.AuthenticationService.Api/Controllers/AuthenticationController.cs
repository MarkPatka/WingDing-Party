using MapsterMapper;
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
    private readonly IMapper _mapper;

    public AuthenticationController(
        ISender sender,
        IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        var applicationResponse = await _sender.Send(command);
        var registerResponse = _mapper.Map<AuthenticationResponse>(applicationResponse);

        //SetTokenCookie(registerResponse.Token);

        //var responseWithoutToken = new
        //{
        //    registerResponse.Id,
        //    registerResponse.FirstName,
        //    registerResponse.LastName,
        //    registerResponse.Email
        //};

        return Ok(registerResponse); //responseWithoutToken
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = _mapper.Map<LoginQuery>(request);
        var applicationResponse = await _sender.Send(query);
        var loginResponse = _mapper.Map<AuthenticationResponse>(applicationResponse);

        //var token = applicationResponse.Token;

        //SetTokenCookie(token);
        //var responseWithoutToken = new
        //{
        //    User = new
        //    {
        //        applicationResponse.User.Id,
        //        applicationResponse.User.FirstName,
        //        applicationResponse.User.LastName,
        //        applicationResponse.User.Email
        //    }
        //};

        return Ok(loginResponse); //responseWithoutToken
    }

    private void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,                // Prevent JavaScript access
            Secure = true,                  // Only send over HTTPS (use false for development on HTTP)
            SameSite = SameSiteMode.Strict, // CSRF protection
            Expires = TimeProvider.System   // Match your token expiration
                .GetUtcNow().AddDays(7) 
        };

        Response.Cookies.Append("jwt_token", token, cookieOptions);
    }

}
