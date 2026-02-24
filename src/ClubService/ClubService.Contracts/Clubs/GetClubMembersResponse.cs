namespace ClubService.Contracts.Clubs;

public record GetClubMembersResponse(Guid UserId, DateTime JoinedAt);