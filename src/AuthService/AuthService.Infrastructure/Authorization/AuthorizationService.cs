using AuthService.Domain.Entities;
using AuthService.Domain.Enumerations;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AuthService.Infrastructure.Authorization;

/// <summary>
/// Queries the DB for user roles and permissions.
/// Results are cached in Redis to avoid hitting the DB on every request.
/// </summary>
internal sealed class AuthorizationService
{
    private readonly AuthDbContext _dbContext;
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public AuthorizationService(AuthDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        string cacheKey = $"auth:roles-{identityId}";
        string? cached = await _cache.GetStringAsync(cacheKey);

        if (cached is not null)
            return JsonSerializer.Deserialize<UserRolesResponse>(cached)!;

        UserRolesResponse roles = await _dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .Select(u => new UserRolesResponse
            {
                UserId = u.Id.Value,
                Roles = u.Roles.ToList()
            })
            .FirstAsync();

        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(roles),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheExpiration });

        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        string cacheKey = $"auth:permissions-{identityId}";
        string? cached = await _cache.GetStringAsync(cacheKey);

        if (cached is not null)
            return JsonSerializer.Deserialize<HashSet<string>>(cached)!;

        // Query: User -> Roles -> Permissions (many-to-many-to-many)
        IReadOnlyCollection<Permission> permissions = await _dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .SelectMany(u => u.Roles.Select(r => r.Permissions))
            .FirstAsync();

        HashSet<string> permissionsSet = [.. permissions.Select(p => p.Name)];

        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(permissionsSet),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheExpiration });

        return permissionsSet;
    }
}