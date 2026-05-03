using AuthService.Application.Common.Interfaces;
using AuthService.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Http;

namespace AuthService.Infrastructure.Authentication;

// todo cutom errors + handler
internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        _httpContextAccessor.HttpContext?.User.GetUserId()
        ?? throw new ApplicationException("User context is unavailable");

    public string IdentityId =>
        _httpContextAccessor.HttpContext?.User.GetIdentityId()
        ?? throw new ApplicationException("User context is unavailable");
}
