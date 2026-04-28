using MediatR;
using UserService.Application.AvatarManagement.Common;

namespace UserService.Application.AvatarManagement.Commands.CreateAvatarCommand;

public record CreateAvatarCommand(
    Stream AvatarStream,
    string FileName,
    string ContentType,
    Guid UserId,
    bool IsDefault,
    bool IsActive)
    : IRequest<CreateAvatarResult>;