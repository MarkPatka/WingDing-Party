namespace UserService.Contracts.UserProfiles;

public record CreateUserProfileResponse(
    Guid Id,
    string DisplayName,
    string Bio,
    Uri? AvatarUri,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);