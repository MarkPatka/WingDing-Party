using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;

namespace ClubService.Application.Persistence.Specifications.Clubs;

public class ClubsByIdsSpec : BaseSpecification<Club>
{
    public ClubsByIdsSpec(IEnumerable<ClubId> clubIds) : base(c => clubIds.Contains(c.Id))
    {
    }
}