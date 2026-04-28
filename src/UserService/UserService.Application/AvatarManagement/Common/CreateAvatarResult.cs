namespace UserService.Application.AvatarManagement.Common;

public record CreateAvatarResult(Guid UseId, Uri? Avatar, bool IsDefault, bool IsActive);