using WingDing_Party.IdentityProvider.Application.Common.Interfaces.Persistence;
using WingDing_Party.IdentityProvider.Domain.Entities;

namespace WingDing_Party.IdentityProvider.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private static readonly List<User> _users = [];

    public void Add(User user)
    {
        _users.Add(user);
    }

    public User? GetUserByEmail(string email)
    {
        return _users.SingleOrDefault(x => x.Email == email);
    }
}
