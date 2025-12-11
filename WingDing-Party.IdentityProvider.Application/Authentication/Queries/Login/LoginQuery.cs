using MediatR;
using WingDing_Party.IdentityProvider.Application.Authentication.Common;

namespace WingDing_Party.IdentityProvider.Application.Authentication.Queries.Login;

public record LoginQuery(string Email, string Password) 
    : IRequest<AuthenticationResult>;
