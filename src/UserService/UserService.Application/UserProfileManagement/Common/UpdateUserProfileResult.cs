namespace UserService.Application.UserProfileManagement.Common;

public record UpdateUserProfileResult(
    string DisplayName,
    string Bio,
    Uri? AvatarUri,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);