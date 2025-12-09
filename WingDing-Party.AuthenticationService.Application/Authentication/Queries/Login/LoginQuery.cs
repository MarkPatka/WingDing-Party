using MediatR;
using WingDing_Party.AuthenticationService.Application.Authentication.Common;

namespace WingDing_Party.AuthenticationService.Application.Authentication.Queries.Login;

public record LoginQuery(string Email, string Password) 
    : IRequest<AuthenticationResult>;
