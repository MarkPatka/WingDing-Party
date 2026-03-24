using UserService.Domain.UserProfileAggregate;

namespace UserService.Application.Persistence.Specifications.UserProfiles;

public class UserByUserNameSpec : BaseSpecification<UserProfile>
{
    public UserByUserNameSpec(string name) : base(c => c.DisplayName == name)
    {
    }
}