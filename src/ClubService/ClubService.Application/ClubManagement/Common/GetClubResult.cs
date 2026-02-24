namespace ClubService.Application.ClubManagement.Common;

public record GetClubResult(Guid ClubId, string Name, string Description, Guid OwnerId, bool IsPublic);