using WingDing_Party.IdentityProvider.Domain.Entities;

namespace WingDing_Party.IdentityProvider.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    void Add(User user);
}
