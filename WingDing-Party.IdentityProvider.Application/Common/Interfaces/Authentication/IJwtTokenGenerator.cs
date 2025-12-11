using WingDing_Party.IdentityProvider.Domain.Entities;

namespace WingDing_Party.IdentityProvider.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);

}
