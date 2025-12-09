using MediatR;
using WingDing_Party.AuthenticationService.Application.Authentication.Common;
using WingDing_Party.AuthenticationService.Application.Common.Errors;
using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Authentication;
using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Persistence;
using WingDing_Party.AuthenticationService.Domain.Entities;

namespace WingDing_Party.AuthenticationService.Application.Authentication.Commands.Register;

public class RegisterCommandHandler :
    IRequestHandler<RegisterCommand, AuthenticationResult>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(
        IJwtTokenGenerator jwtTokenGenerator,
        IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }
    

    public async Task<AuthenticationResult> Handle(
        RegisterCommand command, CancellationToken cancellationToken = default)
    {
        // check exists
        if (_userRepository.GetUserByEmail(command.Email) is not null)
        {
            throw new DuplicateEmailException(
                "Invalid Credentials", 
                System.Net.HttpStatusCode.Conflict);
        }

        // create & persist the user
        var user = new User
        {
            FirstName = command.FirstName,
            LastName  = command.LastName,
            Email     = command.Email,
            Password  = command.Password
        };

        _userRepository.Add(user);

        // create JWT token
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(user, token);
    }
}
