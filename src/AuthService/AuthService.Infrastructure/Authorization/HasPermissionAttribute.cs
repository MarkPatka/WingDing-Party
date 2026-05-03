using Microsoft.AspNetCore.Authorization;

namespace AuthService.Infrastructure.Authorization;

/// <summary>
/// Usage: [HasPermission(Permissions.EventsCreate)]
/// Triggers dynamic policy creation via PermissionAuthorizationPolicyProvider.
/// </summary>
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(permission) // Sets the Policy property to the permission string
    {

    }
}