using Microsoft.AspNetCore.Authorization;

namespace WingDing.Auth.Shared.Authorization;

/// <summary>
/// Usage: [HasPermission(Permissions.EventsCreate)]
/// Triggers dynamic policy creation via PermissionAuthorizationPolicyProvider.
/// </summary>
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    // Sets the Policy property to the permission string
    public HasPermissionAttribute(string permission) : base(permission)  { }
}