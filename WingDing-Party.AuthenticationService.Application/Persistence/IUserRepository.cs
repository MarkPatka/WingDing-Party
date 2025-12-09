using WingDing_Party.AuthenticationService.Domain.Entities;

namespace WingDing_Party.AuthenticationService.Application.Persistence;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    void Add(User user);
}
