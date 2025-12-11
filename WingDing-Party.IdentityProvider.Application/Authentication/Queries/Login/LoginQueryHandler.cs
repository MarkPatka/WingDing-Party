using MediatR;
using WingDing_Party.IdentityProvider.Application.Authentication.Common;
using WingDing_Party.IdentityProvider.Application.Common.Interfaces.Authentication;
using WingDing_Party.IdentityProvider.Application.Common.Interfaces.Persistence;
using WingDing_Party.IdentityProvider.Domain.Entities;

namespace WingDing_Party.IdentityProvider.Application.Authentication.Queries.Login;

public class LoginQueryHandler
      : IRequestHandler<LoginQuery, AuthenticationResult>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public LoginQueryHandler(
        IJwtTokenGenerator jwtTokenGenerator,
        IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<AuthenticationResult> Handle(
        LoginQuery query, CancellationToken cancellationToken = default)
    {
        // check exists
        if (_userRepository.GetUserByEmail(query.Email) is not User user)
        {
            throw new Exception("User doesn`t exists");
        }

        // validate password
        if (user.Password != query.Password)
        {
            throw new Exception("Invalid password");
        }

        // create JWT 
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(user, token);
    }
}
