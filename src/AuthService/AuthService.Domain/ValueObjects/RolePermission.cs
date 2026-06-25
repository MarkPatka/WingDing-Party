using AuthService.Domain.Common.Abstractions;
using AuthService.Domain.ValueObjects.Ids;

namespace AuthService.Domain.ValueObjects;

public sealed class RolePermission : ValueObject
{
    public RoleId RoleId { get; }
    public int PermissionId { get; }

    private RolePermission(RoleId roleId, int permissionId)
        => (RoleId, PermissionId) = (roleId, permissionId);

    public static RolePermission Create(RoleId roleId, int permissionId) =>
        new(roleId, permissionId);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return RoleId;
        yield return PermissionId;
    }
}
