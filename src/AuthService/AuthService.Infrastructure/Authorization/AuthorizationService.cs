using AuthService.Domain.Entities;
using AuthService.Domain.Enumerations;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AuthService.Infrastructure.Authorization;

/// <summary>
/// Queries the DB for user roles and permissions.
/// Uses IDbContextFactory because the DbContext is registered as a factory
/// (AddDbContextFactory in Infrastructure DI). Results are cached in Redis.
/// </summary>
public sealed class AuthorizationService
{
    private readonly IDbContextFactory<AuthDbContext> _dbContextFactory;
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public AuthorizationService(IDbContextFactory<AuthDbContext> dbContextFactory, IDistributedCache cache)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        string cacheKey = $"auth:roles-{identityId}";
        string? cached = await _cache.GetStringAsync(cacheKey);

        if (cached is not null)
            return JsonSerializer.Deserialize<UserRolesResponse>(cached)!;

        await using AuthDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        UserRolesResponse? roles = await dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .Select(u => new UserRolesResponse 
            {
                UserId = u.Id.Value, 
                Roles = u.Roles.ToList()
            })
            .FirstOrDefaultAsync();

        if (roles is null)
            return new UserRolesResponse { UserId = Guid.Empty, Roles = [] };

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

        await using AuthDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        var permissions = await dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .SelectMany(u => u.Roles.SelectMany(r => r.Permissions))
            .ToListAsync();

        HashSet<string> permissionsSet = [.. permissions.Select(p => p.Name)];

        if (permissionsSet.Count != 0)
        {
            await _cache.SetStringAsync(cacheKey,
                JsonSerializer.Serialize(permissionsSet),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheExpiration });
        }
        return permissionsSet;
    }
}
