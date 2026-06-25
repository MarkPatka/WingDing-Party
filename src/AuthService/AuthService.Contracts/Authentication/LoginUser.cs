namespace AuthService.Contracts.Authentication;

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(string AccessToken);