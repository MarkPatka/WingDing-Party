using WingDing_Party.AuthenticationService.Domain.Entities;

namespace WingDing_Party.AuthenticationService.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);

}
