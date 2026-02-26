namespace ClubService.Contracts.Clubs;

public record GetClubResponse(Guid ClubId, string Name, string Description, Guid OwnerId, bool IsPublic);