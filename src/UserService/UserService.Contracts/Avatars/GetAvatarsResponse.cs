namespace UserService.Contracts.Avatars;

public record GetAvatarsResponse(Uri? Avatar, bool IsDefault, bool IsActive);