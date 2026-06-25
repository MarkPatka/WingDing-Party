using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.Persistence.Specifications.UserProfiles;

public class UserByUserIdSpec : BaseSpecification<UserProfile>
{
    public UserByUserIdSpec(UserId userId) : base(c => c.Id == userId)
    {
    }
}