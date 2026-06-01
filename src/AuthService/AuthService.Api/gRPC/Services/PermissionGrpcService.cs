using Grpc.Core;
using WingDing.Auth.Shared.Grpc;
using WingDing.Auth.Shared.Services;

namespace AuthService.Api.gRPC.Services;

/// <summary>
/// gRPC implementation of Permission Oracle.
/// Consumed by downstream services (Event, Club, User) over HTTP/2.
/// NOT exposed to frontend — only reachable within Docker network.
/// </summary>
public sealed class PermissionGrpcService : PermissionOracle.PermissionOracleBase
{
    private readonly IPermissionService _permissionService;

    public PermissionGrpcService(IPermissionService permissionService)
        => _permissionService = permissionService;

    public override async Task<PermissionResponse> GetPermissions(
        PermissionRequest request, ServerCallContext context)
    {
        HashSet<string> permissions = await _permissionService
            .GetPermissionsForUserAsync(request.IdentityId);

        var response = new PermissionResponse();
        response.Permissions.AddRange(permissions);
        return response;
    }

    public override async Task<RolesResponse> GetRoles(
        RolesRequest request, ServerCallContext context)
    {
        UserRolesDto roles = await _permissionService
            .GetRolesForUserAsync(request.IdentityId);

        return new RolesResponse
        {
            UserId = roles.UserId.ToString(),
            RoleNames = { roles.RoleNames }
        };
    }
}
