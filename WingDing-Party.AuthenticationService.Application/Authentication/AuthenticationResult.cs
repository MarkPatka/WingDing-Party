using WingDing_Party.AuthenticationService.Domain.Entities;

namespace WingDing_Party.AuthenticationService.Application.Authentication;

public record AuthenticationResult(
    User User, string Token);


