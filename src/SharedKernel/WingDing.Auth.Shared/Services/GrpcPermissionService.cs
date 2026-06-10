using Microsoft.Extensions.Caching.Memory;
using WingDing.Auth.Shared.Grpc;

namespace WingDing.Auth.Shared.Services;

/// <summary>
/// Calls AuthService gRPC endpoint (PermissionOracle) to get permissions and roles.
/// Uses Protobuf over HTTP/2 -- ~5x smaller payloads than JSON, multiplexed connection.
/// Includes in-memory cache (30s TTL) to avoid hammering AuthService.
/// AuthService itself also caches in Redis (5 min TTL), so we get two cache layers.
/// </summary>
public sealed class GrpcPermissionService : IPermissionService
{
    private readonly PermissionOracle.PermissionOracleClient _client;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    public GrpcPermissionService(
        PermissionOracle.PermissionOracleClient client,
        IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        string cacheKey = $"grpc:perms:{identityId}";

        if (_cache.TryGetValue(cacheKey, out HashSet<string>? cached) && cached is not null)
            return cached;

        // gRPC call to AuthService: PermissionOracle.GetPermissions()
        var response = await _client.GetPermissionsAsync(
            new PermissionRequest { IdentityId = identityId });

        HashSet<string> permissions = [.. response.Permissions];

        _cache.Set(cacheKey, permissions, CacheTtl);
        return permissions;
    }

    public async Task<UserRolesDto> GetRolesForUserAsync(string identityId)
    {
        string cacheKey = $"grpc:roles:{identityId}";

        if (_cache.TryGetValue(cacheKey, out UserRolesDto? cached) && cached is not null)
            return cached;

        // gRPC call to AuthService: PermissionOracle.GetRoles()
        var response = await _client.GetRolesAsync(
            new RolesRequest { IdentityId = identityId });

        var roles = new UserRolesDto
        {
            UserId = Guid.Parse(response.UserId),
            RoleNames = [.. response.RoleNames]
        };

        _cache.Set(cacheKey, roles, CacheTtl);
        return roles;
    }
}