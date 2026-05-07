namespace UserService.Application.AvatarManagement.Common;

public record CreateAvatarResult(Guid Id, Guid UserId, Uri? Avatar, bool IsDefault, bool IsActive);