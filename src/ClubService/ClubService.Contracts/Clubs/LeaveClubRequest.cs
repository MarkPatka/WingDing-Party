namespace ClubService.Contracts.Clubs;

public record LeaveClubRequest(Guid UserId, Guid ClubId);