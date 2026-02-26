using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;

namespace ClubService.Application.Persistence.Specifications.Clubs;

public class ClubsByUserSpec : BaseSpecification<Club>
{
    public ClubsByUserSpec(UserId userId) : base(c => c.ClubMembers.Any(c => c.UserId == userId))
    {
        ApplyNoTracking();
    }
}