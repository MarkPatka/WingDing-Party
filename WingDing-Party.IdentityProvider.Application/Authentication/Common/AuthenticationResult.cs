using WingDing_Party.IdentityProvider.Domain.Entities;

namespace WingDing_Party.IdentityProvider.Application.Authentication.Common;

public record AuthenticationResult(User User, string Token);
