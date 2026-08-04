using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using WingDing.SharedKernel.Auth.Services;

namespace WingDing.SharedKernel.Auth.Authorization;

/// <summary>
/// Enriches the ClaimsPrincipal with data from the auth DB (via IPermissionService).
///
/// In AuthService — LocalPermissionService queries the DB directly.
/// In downstream services — GrpcPermissionService calls AuthService over gRPC.
/// </summary>
internal sealed class RemoteClaimsTransformation(IServiceProvider serviceProvider) : IClaimsTransformation
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
        IPermissionService permissionService = scope.ServiceProvider
            .GetRequiredService<IPermissionService>();

        string identityId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new ApplicationException("User identity is unavailable");

        UserRolesDto userRoles = await permissionService.GetRolesForUserAsync(identityId);

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(
            new Claim(JwtRegisteredClaimNames.Sub, userRoles.UserId.ToString()));

        foreach (string roleName in userRoles.RoleNames)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));
        }

        principal.AddIdentity(claimsIdentity);
        return principal;
    }
}
