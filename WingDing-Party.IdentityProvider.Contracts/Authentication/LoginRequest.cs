namespace WingDing_Party.IdentityProvider.Contracts.Authentication;

public record LoginRequest(
    string Email,
    string Password);
