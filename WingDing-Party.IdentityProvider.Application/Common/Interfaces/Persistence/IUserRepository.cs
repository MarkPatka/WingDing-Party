using WingDing_Party.IdentityProvider.Domain.UserAggregate;

namespace WingDing_Party.IdentityProvider.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    void Add(User user);
}
