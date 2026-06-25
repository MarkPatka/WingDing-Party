namespace UserService.Application.AvatarManagement.Common;

public record UpdateAvatarResult(Guid Id, Guid UserId, Uri? Avatar, bool IsDefault, bool IsActive);