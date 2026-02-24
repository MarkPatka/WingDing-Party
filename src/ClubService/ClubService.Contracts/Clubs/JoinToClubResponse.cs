namespace ClubService.Contracts.Clubs;

public record JoinToClubResponse(Guid ClubId, Guid UserId, DateTime JoinedAt);