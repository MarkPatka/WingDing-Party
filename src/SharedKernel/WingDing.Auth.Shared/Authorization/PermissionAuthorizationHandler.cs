using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using WingDing.Auth.Shared.Services;

namespace WingDing.Auth.Shared.Authorization;

/// <summary>
/// The actual permission check. Called by ASP.NET Core's authorization middleware.
/// Uses IPermissionService — resolved as LocalPermissionService in AuthService (DB direct)
/// or GrpcPermissionService in downstream services (gRPC call to AuthService).
/// </summary>
internal sealed class PermissionAuthorizationHandler(IServiceProvider serviceProvider)
        : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
            return;

        using IServiceScope scope = _serviceProvider.CreateScope();
        IPermissionService permissionService = scope.ServiceProvider
            .GetRequiredService<IPermissionService>();

        string identityId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new ApplicationException("User identity is unavailable");

        HashSet<string> permissions = await permissionService
            .GetPermissionsForUserAsync(identityId);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
