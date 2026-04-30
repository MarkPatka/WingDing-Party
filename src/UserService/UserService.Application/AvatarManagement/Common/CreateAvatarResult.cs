namespace UserService.Application.AvatarManagement.Common;

public record CreateAvatarResult(Guid UserId, Uri? Avatar, bool IsDefault, bool IsActive);