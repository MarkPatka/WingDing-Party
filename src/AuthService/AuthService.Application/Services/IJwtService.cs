namespace AuthService.Application.Services;

public interface IJwtService
{
    Task<string?> GetAccessTokenAsync(string email, string password, CancellationToken ct = default);
}
