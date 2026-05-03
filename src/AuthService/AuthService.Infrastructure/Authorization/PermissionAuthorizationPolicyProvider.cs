using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Authorization;

/// <summary>
/// Dynamically creates authorization policies on-the-fly.
///
/// Flow:
/// 1. Controller has [HasPermission("events:create")]
/// 2. ASP.NET asks: "Do you have a policy named 'events:create'?"
/// 3. Base class says: "No, I don't have a pre-registered one."
/// 4. We create one: new policy that requires PermissionRequirement("events:create")
/// 5. Cache it in AuthorizationOptions so we don't rebuild next time.
/// </summary>
internal sealed class PermissionAuthorizationPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authorizationOptions;

    public PermissionAuthorizationPolicyProvider(
        IOptions<AuthorizationOptions> options) : base(options)
    {
        _authorizationOptions = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // First check if there's an existing pre-registered policy
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);
        if (policy is not null)
            return policy;

        // Create a new policy dynamically with PermissionRequirement
        AuthorizationPolicy permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        // Cache it for subsequent requests
        _authorizationOptions.AddPolicy(policyName, permissionPolicy);

        return permissionPolicy;
    }
}
