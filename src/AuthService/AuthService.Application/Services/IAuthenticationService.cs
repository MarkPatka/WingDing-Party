using AuthService.Domain.Entities;

namespace AuthService.Application.Services;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(User user, string password, CancellationToken ct = default);
}
