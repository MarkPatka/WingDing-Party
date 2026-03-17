using ClubService.Domain.ClubAggregate.ValueObjects;
using ClubService.Domain.Common.Abstract;

namespace ClubService.Domain.ClubAggregate;

public sealed class ClubMember : Entity<int>
{
    public ClubId ClubId { get; private set; }
    public UserId UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private ClubMember() { }

    private ClubMember(
        ClubId clubId,
        UserId userId,
        DateTime joinedAt)
    {
        ClubId = clubId;
        UserId = userId;
        JoinedAt = joinedAt;
    }

    public static ClubMember Create(
        ClubId clubId,
        UserId userId,
        DateTime joinedAt)

    {
        return new ClubMember(clubId, userId, joinedAt);
    }
}