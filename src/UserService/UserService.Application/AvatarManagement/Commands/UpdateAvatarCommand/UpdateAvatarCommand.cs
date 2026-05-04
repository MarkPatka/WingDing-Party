using MediatR;
using UserService.Application.AvatarManagement.Common;

namespace UserService.Application.AvatarManagement.Commands.UpdateAvatarCommand;

public record UpdateAvatarCommand(
    Guid AvatarId,
    Guid UserId,
    bool IsDefault,
    bool IsActive)
    : IRequest<UpdateAvatarResult>;