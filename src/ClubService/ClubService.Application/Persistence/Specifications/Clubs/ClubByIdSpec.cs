using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;

namespace ClubService.Application.Persistence.Specifications.Clubs;

public class ClubByIdSpec : BaseSpecification<Club>
{
    public ClubByIdSpec(ClubId clubId) : base(c => c.Id == clubId)
    {
    }
}