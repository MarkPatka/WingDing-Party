namespace WingDing.SharedKernel.Auth.Services;

/// <summary>
/// Abstraction for permission checking.
/// Two implementations:
///   - LocalPermissionService (used by AuthService itself - queries DB directly)
///   - GrpcPermissionService (used by all other services - calls AuthService via gRPC)
/// </summary>
public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsForUserAsync(string identityId);
    Task<UserRolesDto> GetRolesForUserAsync(string identityId);
}

public sealed class UserRolesDto
{
    public Guid UserId { get; init; }
    public List<string> RoleNames { get; init; } = [];
}
