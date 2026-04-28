using AuthService.Domain.Common.Abstractions;
using System.Data;

namespace AuthService.Domain.Enumerations;

public sealed class RoleType : Enumeration
{
    public static readonly RoleType Guest      = new(1, "Guest");
    public static readonly RoleType Registered = new(2, "Registered");
    public static readonly RoleType Moderator  = new(3, "Moderator");
    public static readonly RoleType Admin      = new(4, "Administrator");

    public RoleType(int id, string name, string? description = null) 
        : base(id, name, description) { }
}
