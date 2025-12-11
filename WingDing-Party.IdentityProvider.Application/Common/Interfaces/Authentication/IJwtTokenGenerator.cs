using WingDing_Party.IdentityProvider.Domain.UserAggregate;

namespace WingDing_Party.IdentityProvider.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);

}
