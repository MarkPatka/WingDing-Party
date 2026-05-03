using AuthService.Application.Services;
using AuthService.Infrastructure.Common.Configuration;
using AuthService.Infrastructure.Common.Dto;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Services;

internal sealed class JwtService : IJwtService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;

    public JwtService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    /// <summary>
    /// Resource Owner Password Credentials flow.
    /// ONLY for development/testing. Production should use authorization_code + PKCE.
    /// </summary>
    public async Task<string?> GetAccessTokenAsync(
        string email, string password, CancellationToken ct = default)
    {
        var parameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _keycloakOptions.AuthClientId),
            new("client_secret", _keycloakOptions.AuthClientSecret),
            new("scope", "openid email"),
            new("grant_type", "password"),
            new("username", email),
            new("password", password)
        };

        using var content = new FormUrlEncodedContent(parameters);
        HttpResponseMessage response = await _httpClient.PostAsync("", content, ct);

        if (!response.IsSuccessStatusCode)
            return null;

        var token = await response.Content.ReadFromJsonAsync<AuthorizationToken>(ct);
        return token?.AccessToken;
    }
}
