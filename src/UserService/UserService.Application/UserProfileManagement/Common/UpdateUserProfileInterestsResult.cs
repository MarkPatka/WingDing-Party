namespace UserService.Application.UserProfileManagement.Common;

public record UpdateUserProfileInterestsResult(IEnumerable<string> Interests);