using WingDing_Party.IdentityProvider.Domain.UserAggregate;

namespace WingDing_Party.IdentityProvider.Application.Authentication.Common;

public record AuthenticationResult(User User, string Token);
