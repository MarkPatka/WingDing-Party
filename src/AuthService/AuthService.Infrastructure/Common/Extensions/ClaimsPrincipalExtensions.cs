using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace AuthService.Infrastructure.Common.Extensions;


// todo custom errors + handler
internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the application-level User ID (from our DB, injected via CustomClaimsTransformation).
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(userId, out Guid parsedUserId)
            ? parsedUserId
            : throw new ApplicationException("User id is unavailable");
    }

    /// <summary>
    /// Gets the Keycloak Identity ID (the original 'sub' claim from the JWT).
    /// </summary>
    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new ApplicationException("User identity is unavailable");
    }
}
