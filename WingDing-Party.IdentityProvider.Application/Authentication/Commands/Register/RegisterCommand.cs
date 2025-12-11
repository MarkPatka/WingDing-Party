using MediatR;
using WingDing_Party.IdentityProvider.Application.Authentication.Common;

namespace WingDing_Party.IdentityProvider.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<AuthenticationResult>;
