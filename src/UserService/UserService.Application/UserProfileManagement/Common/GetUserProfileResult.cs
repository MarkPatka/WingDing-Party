using UserService.Domain.UserProfileAggregate.Entities;

namespace UserService.Application.UserProfileManagement.Common;

public record GetUserProfileResult(
    string DisplayName,
    string Bio,
    IReadOnlyList<Avatar> Avatars,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate
);