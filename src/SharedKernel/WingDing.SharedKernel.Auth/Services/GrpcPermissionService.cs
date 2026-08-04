using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WingDing.Auth.Shared.Grpc;

namespace WingDing.SharedKernel.Auth.Services;

/// <summary>
/// Calls AuthService gRPC endpoint (PermissionOracle) to get permissions and roles.
/// Uses Protobuf over HTTP/2 -- ~5x smaller payloads than JSON, multiplexed connection.
/// Includes in-memory cache (30s TTL) to avoid hammering AuthService.
/// AuthService itself also caches in Redis (5 min TTL), so we get two cache layers.
/// </summary>
public sealed class GrpcPermissionService : IPermissionService
{
    private readonly PermissionOracle.PermissionOracleClient _client;
    private readonly ILogger<GrpcPermissionService> _logger;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    public GrpcPermissionService(
        PermissionOracle.PermissionOracleClient client,
        ILogger<GrpcPermissionService> logger,
        IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
        _logger = logger;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        string cacheKey = $"grpc:perms:{identityId}";

        if (_cache.TryGetValue(cacheKey, out HashSet<string>? cached) && cached is not null)
            return cached;
        try
        {
            // gRPC call to AuthService: PermissionOracle.GetPermissions()
            var response = await _client.GetPermissionsAsync(
                new PermissionRequest { IdentityId = identityId });

            HashSet<string> permissions = [.. response.Permissions];

            if (permissions.Count != 0)
            {
                _cache.Set(cacheKey, permissions, CacheTtl);
            }
            return permissions;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            _logger.LogError(ex, "Auth service unavailable for identity {IdentityId}", identityId);
            throw; 
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            _logger.LogWarning("Identity {IdentityId} not found", identityId);
            return []; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for identity {IdentityId}", identityId);
            throw;
        }
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