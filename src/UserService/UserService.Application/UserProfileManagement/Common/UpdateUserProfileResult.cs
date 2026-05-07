namespace UserService.Application.UserProfileManagement.Common;

public record UpdateUserProfileResult(
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);