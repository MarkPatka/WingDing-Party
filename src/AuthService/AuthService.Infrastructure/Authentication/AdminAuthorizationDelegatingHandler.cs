using AuthService.Infrastructure.Common.Configuration;
using AuthService.Infrastructure.Common.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Authentication;

/// <summary>
/// DelegatingHandler that automatically attaches a service-account Bearer token
/// when calling Keycloak Admin REST API (e.g., creating users).
/// Uses client_credentials flow with wingding-admin-client.
/// </summary>
internal sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly KeycloakOptions _keycloakOptions;

    public AdminAuthorizationDelegatingHandler(IOptions<KeycloakOptions> keycloakOptions)
    {
        _keycloakOptions = keycloakOptions.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AuthorizationToken token = await GetAuthorizationToken(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme, token.AccessToken);

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return response;
    }

    private async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken ct)
    {
        var parameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _keycloakOptions.AdminClientId),
            new("client_secret", _keycloakOptions.AdminClientSecret),
            new("scope", "openid email"),
            new("grant_type", "client_credentials")
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_keycloakOptions.TokenUrl))
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        HttpResponseMessage response = await base.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AuthorizationToken>(ct)
               ?? throw new ApplicationException("Failed to deserialize Keycloak token response");
    }
}