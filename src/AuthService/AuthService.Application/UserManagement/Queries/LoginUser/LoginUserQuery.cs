using AuthService.Application.UserManagement.Common;
using MediatR;

namespace AuthService.Application.UserManagement.Queries.LoginUser;

public sealed record LoginUserQuery(string Email, string Password) : IRequest<LoginUserResult?>;