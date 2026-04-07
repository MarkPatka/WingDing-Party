using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.Persistence.Specifications.UserProfiles;

public class UsersInterestsByUserIdSpec : BaseSpecification<UserProfile>
{
    public UsersInterestsByUserIdSpec(UserId userId) : base(c => c.Id == userId)
    {
    }
}