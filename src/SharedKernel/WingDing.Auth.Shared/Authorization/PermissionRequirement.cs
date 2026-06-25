using Microsoft.AspNetCore.Authorization;

namespace WingDing.Auth.Shared.Authorization;

internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}