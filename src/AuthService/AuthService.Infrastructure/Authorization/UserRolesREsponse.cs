using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Authorization;

public sealed record UserRolesResponse
{
    public Guid UserId { get; init; }
    public List<Role> Roles { get; init; } = [];
}
