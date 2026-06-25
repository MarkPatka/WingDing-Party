using AuthService.Application.Services;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Common.Dto;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Services;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> RegisterAsync(
        User user, string password, CancellationToken ct = default)
    {
        var model = new UserRepresentationModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Email,
            Enabled = true,
            EmailVerified = true,
            Credentials =
            [
                new CredentialRepresentationModel
                {
                    Value = password,
                    Temporary = false
                }
            ]
        };

        // POST to Keycloak Admin API: /admin/realms/wingding-party/users
        HttpResponseMessage response = await _httpClient
            .PostAsJsonAsync("users", model, ct);
        
        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync(ct);
        
            throw new InvalidOperationException(
                $"Keycloak user creation failed: {(int)response.StatusCode} {body}");
        }

        return ExtractIdentityIdFromLocationHeader(response);
    }

    private static string ExtractIdentityIdFromLocationHeader(HttpResponseMessage response)
    {
        const string usersSegment = "users/";

        string? location = response.Headers.Location?.PathAndQuery
            ?? throw new InvalidOperationException("Keycloak did not return Location header");

        int idx = location.IndexOf(usersSegment, StringComparison.OrdinalIgnoreCase);
        return location[(idx + usersSegment.Length)..];
    }
}
