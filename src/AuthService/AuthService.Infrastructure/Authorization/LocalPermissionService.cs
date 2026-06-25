using WingDing.Auth.Shared.Services;

namespace AuthService.Infrastructure.Authorization;

/// <summary>
/// AuthService's own implementation of IPermissionService — queries the DB directly
/// (this service owns authdb). Other services use GrpcPermissionService instead.
/// </summary>
internal sealed class LocalPermissionService(AuthorizationService authorizationService) : IPermissionService
{
    private readonly AuthorizationService _authorizationService = authorizationService;

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        return await _authorizationService.GetPermissionsForUserAsync(identityId);
    }

    public async Task<UserRolesDto> GetRolesForUserAsync(string identityId)
    {
        UserRolesResponse response = await _authorizationService.GetRolesForUserAsync(identityId);
        return new UserRolesDto
        {
            UserId = response.UserId,
            RoleNames = response.Roles.Select(r => r.RoleType.Name).ToList()
        };
    }
}
