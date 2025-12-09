using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Authentication;
using WingDing_Party.AuthenticationService.Application.Persistence;
using WingDing_Party.AuthenticationService.Domain.Entities;

namespace WingDing_Party.AuthenticationService.Application.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(
        IJwtTokenGenerator jwtTokenGenerator,
        IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public AuthenticationResult Login(string email, string password)
    {
        // check exists
        if (_userRepository.GetUserByEmail(email) is not User user)
        {
            throw new Exception("User doesn`t exists");
        }

        // validate password
        if (user.Password != password)
        {
            throw new Exception("Invalid password");
        }

        // create JWT 
        var token = _jwtTokenGenerator
            .GenerateToken(user);

        return new AuthenticationResult(
            user, token);
    }

    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
        // check exists
        if (_userRepository.GetUserByEmail(email) is not null)
        {
            throw new Exception("User already exists");
        }

        // create & persist the user
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

        _userRepository.Add(user);

        // create JWT token
        var token = _jwtTokenGenerator
            .GenerateToken(user);

        return new AuthenticationResult(user, token);
    }
}
