using AuthService.Application.UserManagement.Common;
using MediatR;

namespace AuthService.Application.UserManagement.Command.AssignRole;

public sealed record AssignRoleCommand(Guid UserId, string Role) : IRequest<AssignRoleResult>;
