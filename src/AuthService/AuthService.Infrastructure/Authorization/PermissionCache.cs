using AuthService.Application.Services;
using AuthService.Domain.Common.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthService.Infrastructure.Authorization;

/// <summary>
/// Invalidates a user's cached roles/permissions in Redis.
/// Keys MUST match the ones written by <see cref="AuthorizationService"/>.
/// </summary>
internal sealed class PermissionCache(IDistributedCache cache) : IPermissionCache
{
    private readonly IDistributedCache _cache = cache;

    public async Task InvalidateAsync(string identityId)
    {
        await _cache.RemoveAsync($"auth:roles-{identityId}");
        await _cache.RemoveAsync($"auth:permissions-{identityId}");
    }
}
