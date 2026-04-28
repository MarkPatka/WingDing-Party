using UserService.Domain.UserProfileAggregate.Entities;

namespace UserService.Application.UserProfileManagement.Common;

public record CreateUserProfileResult(
    Guid Id,
    string DisplayName,
    string Bio,
    Avatar? Avatar,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);