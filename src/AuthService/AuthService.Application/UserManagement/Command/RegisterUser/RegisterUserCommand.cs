using AuthService.Application.UserManagement.Common;
using MediatR;

namespace AuthService.Application.UserManagement.Command.RegisterUser;

public sealed record RegisterUserCommand(string FirstName, string LastName, string Email, string Password) : IRequest<RegisterUserResult>;
