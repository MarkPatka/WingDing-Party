namespace UserService.Application.UserProfileManagement.Common;

public record GetUserProfileResult(
    string DisplayName,
    string Bio,
    Uri? AvatarUri,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate
    );