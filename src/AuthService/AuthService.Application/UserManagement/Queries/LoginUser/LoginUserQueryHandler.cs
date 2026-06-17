using AuthService.Application.Services;
using AuthService.Application.UserManagement.Common;
using MediatR;

namespace AuthService.Application.UserManagement.Queries.LoginUser;

public class LoginUserQueryHandler(IJwtService jwtService) : IRequestHandler<LoginUserQuery, LoginUserResult?>
{
    private readonly IJwtService _jwtService = jwtService;

    public async Task<LoginUserResult?> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        string? token = await _jwtService.GetAccessTokenAsync(request.Email, request.Password, cancellationToken);
        return token is null ? null : new LoginUserResult(token);
    }
}
