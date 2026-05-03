using System.Security.Claims;
using AuthService.Domain.Entities   ;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AuthService.Infrastructure.Authorization;

/// <summary>
/// Enriches the ClaimsPrincipal with data from OUR database.
///
/// Problem: Keycloak JWT has only Keycloak's sub claim. Our API needs:
/// - Our internal User ID (for DB queries)
/// - Our Role names (for [Authorize(Roles = "Admin")])
///
/// Solution: After JWT validation, this transformer runs and adds:
/// 1. Overrides 'sub' claim with our DB User.Id
/// 2. Adds ClaimTypes.Role for each role from our DB
///
/// This runs ONCE per request (ASP.NET calls it during authentication).
/// </summary>
internal sealed class CustomClaimsTransformation(IServiceProvider serviceProvider) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not { IsAuthenticated: true } ||
            (principal.HasClaim(c => c.Type == ClaimTypes.Role) &&
             principal.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub)))
        {
            return principal;
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        AuthorizationService authService = scope.ServiceProvider
            .GetRequiredService<AuthorizationService>();

        string identityId = principal.GetIdentityId();
        UserRolesResponse userRoles = await authService.GetRolesForUserAsync(identityId);

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(
            new Claim(JwtRegisteredClaimNames.Sub, userRoles.UserId.ToString()));

        foreach (Role role in userRoles.Roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleType.Name));
        }

        principal.AddIdentity(claimsIdentity);
        return principal;
    }
}