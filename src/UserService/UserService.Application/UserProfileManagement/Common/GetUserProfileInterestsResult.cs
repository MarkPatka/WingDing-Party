namespace UserService.Application.UserProfileManagement.Common;

public record GetUserProfileInterestsResult(IEnumerable<string> Interests);