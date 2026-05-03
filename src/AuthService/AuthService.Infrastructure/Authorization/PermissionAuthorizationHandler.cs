using AuthService.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Authorization;

/// <summary>
/// The actual permission check. Called by ASP.NET Core's authorization middleware.
///
/// Flow:
/// 1. PermissionAuthorizationPolicyProvider created a policy with PermissionRequirement
/// 2. ASP.NET Core calls this handler: "Does this user satisfy PermissionRequirement?"
/// 3. We extract the Keycloak identity ID from the JWT claims
/// 4. We query our DB (via AuthorizationService) for that user's permissions
/// 5. If the required permission exists -> context.Succeed() -> request proceeds
/// 6. If not -> no Succeed() called -> 403 Forbidden
/// </summary>
internal sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Step 1: Check if user is authenticated (JWT was valid)
        if (context.User.Identity is not { IsAuthenticated: true })
            return;

        // Step 2: Create a scope (handler is Transient, but AuthorizationService is Scoped)
        using IServiceScope scope = _serviceProvider.CreateScope();
        AuthorizationService authService = scope.ServiceProvider
            .GetRequiredService<AuthorizationService>();

        // Step 3: Get Keycloak identity ID from JWT claims
        string identityId = context.User.GetIdentityId();

        // Step 4: Get all permissions for this user from our DB (cached in Redis)
        HashSet<string> permissions = await authService
            .GetPermissionsForUserAsync(identityId);

        // Step 5: Check if user has the required permission
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        // If not found, handler simply returns without calling Succeed()
        // ASP.NET Core interprets this as 403 Forbidden
    }
}