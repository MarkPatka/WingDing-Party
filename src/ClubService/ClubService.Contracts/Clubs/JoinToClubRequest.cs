namespace ClubService.Contracts.Clubs;

public record JoinToClubRequest(Guid UserId, Guid ClubId);