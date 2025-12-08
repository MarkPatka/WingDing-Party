using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Authentication;

namespace WingDing_Party.AuthenticationService.Application.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public AuthenticationResult Login(string email, string password)
    {
        return new AuthenticationResult(Guid.NewGuid(), "", "", email, password);
    }

    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
        // check exists

        // create user

        // create JWT token
        Guid userId = Guid.NewGuid();
        var token = _jwtTokenGenerator.GenerateToken(userId, firstName, lastName);

        return new AuthenticationResult(
            Guid.NewGuid(), "", "", email, password); 
    }
}
