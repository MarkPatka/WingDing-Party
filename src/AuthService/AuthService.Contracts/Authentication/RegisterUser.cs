namespace AuthService.Contracts.Authentication;

public record RegisterUserRequest(string FirstName, string? LastName, string Email, string Password);
public record RegisterUserResponse(Guid UserId, string Email);
