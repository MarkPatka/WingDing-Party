using System.Data;
using WingDing_Party.IdentityProvider.Domain.Common.Abstract;
using WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.Entities;

//public sealed class UserRole : Entity<UserRoleId>
//{
//    public UserId UserId { get; private set; }
//    public RoleId RoleId { get; private set; }
//    public DateTime AssignedAt { get; private set; }

//    public Role? Role { get; private set; }

//    private UserRole() { } // EF Core

//    private UserRole(UserRoleId id, UserId userId, RoleId roleId) : base(id)
//    {
//        UserId = userId;
//        RoleId = roleId;
//        AssignedAt = DateTime.UtcNow;
//    }

//    public static UserRole Create(UserId userId, RoleId roleId)
//    {
//        return new UserRole(UserRoleId.CreateUnique(), userId, roleId);
//    }
//}

