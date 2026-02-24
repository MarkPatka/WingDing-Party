namespace ClubService.Application.ClubManagement.Common;

public record UpdateClubResult(
    Guid ClubId,
    string Name,
    string Description,
    IEnumerable<string> Interests,
    Guid OwnerId,
    bool IsPublic);