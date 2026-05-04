using MediatR;
using UserService.Application.AvatarManagement.Common;

namespace UserService.Application.AvatarManagement.Commands.DeleteAvatarCommand;

public record DeleteAvatarCommand(Guid AvatarId, Guid UserId) : IRequest<DeleteAvatarResult>;