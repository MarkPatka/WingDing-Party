namespace AuthService.Application.Services;

public interface IPermissionCache
{
    public Task InvalidateAsync(string IdentityId);
}
