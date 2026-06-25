using AuthService.Domain.Common.Abstractions;
using AuthService.Domain.Enumerations;
using AuthService.Domain.ValueObjects.Ids;
using System.Text.Json.Serialization;

namespace AuthService.Domain.Entities;

public sealed class Role : Entity<RoleId>
{
    private readonly List<User> _users = [];
    private readonly List<Permission> _permissions = [];

    public IReadOnlyCollection<User> Users => [.. _users];
    public IReadOnlyCollection<Permission> Permissions => [.. _permissions];
    public RoleType RoleType { get; private set; } = null!;

    private Role() { }
    
    [JsonConstructor]
    private Role(RoleType roleType) : base(RoleId.Create(roleType.Id)) 
    { 
        RoleType = roleType; 
    }

    public static Role Create(RoleType role) => new(role);
}
