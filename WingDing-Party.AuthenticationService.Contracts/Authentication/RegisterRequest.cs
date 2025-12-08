namespace WingDing_Party.AuthenticationService.Contracts.Authentication;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);
