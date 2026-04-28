using UserService.Domain.UserProfileAggregate.Entities;

namespace UserService.Contracts.UserProfiles;

public record CreateUserProfileResponse(
    Guid Id,
    string DisplayName,
    string Bio,
    Avatar? Avatar,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);