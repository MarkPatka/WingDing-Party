namespace UserService.Application.UserProfileManagement.Common;

public record CreateUserProfileResult(
    Guid Id,
    string DisplayName,
    string Bio,
    Uri? AvatarUri,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);