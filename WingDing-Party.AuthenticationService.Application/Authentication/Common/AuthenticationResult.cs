using WingDing_Party.AuthenticationService.Domain.Entities;

namespace WingDing_Party.AuthenticationService.Application.Authentication.Common;

public record AuthenticationResult(User User, string Token);
