namespace WingDing_Party.AuthenticationService.Contracts.Authentication;

public record LoginRequest(
    string Email,
    string Password);
