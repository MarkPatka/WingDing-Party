using AuthService.Domain.Common.Abstractions;
using AuthService.Domain.Entities;
using AuthService.Domain.Enumerations;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.ModelsConfiguration;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value, 
                value => RoleId.Create(value));

        builder.Property(r => r.RoleType)
            .HasConversion(
                rt => rt.Name,
                value => Enumeration.GetFromName<RoleType>(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(r => r.Users)
            .WithMany(u => u.Roles)
            .UsingEntity(j => j.ToTable("user_roles"));

        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>("role_permissions",
                j => j.HasOne<Permission>().WithMany().HasForeignKey(rp => rp.PermissionId),
                j => j.HasOne<Role>().WithMany().HasForeignKey(rp => rp.RoleId),
                j =>
                {
                    j.Property(rp => rp.RoleId)
                        .HasConversion(id => id.Value, value => RoleId.Create(value));
                    j.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                    j.ToTable("role_permissions");
                    j.HasData(DefaultRolePermissions);
                });

        builder.Navigation(r => r.Permissions)
            .HasField("_permissions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(r => r.Users)
            .HasField("_users")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(
            new { Id = RoleId.Create(RoleType.Guest.Id),      RoleType = RoleType.Guest },
            new { Id = RoleId.Create(RoleType.Registered.Id), RoleType = RoleType.Registered },
            new { Id = RoleId.Create(RoleType.Moderator.Id),  RoleType = RoleType.Moderator },
            new { Id = RoleId.Create(RoleType.Admin.Id),      RoleType = RoleType.Admin });
    }

    private static readonly object[] DefaultRolePermissions =
    [
        // Guest: read-only
        new { RoleId = RoleId.Create(RoleType.Guest.Id),     PermissionId = Permission.EventsRead.Id },
        new { RoleId = RoleId.Create(RoleType.Guest.Id),     PermissionId = Permission.ClubsRead.Id },

        // Registered: read + own profile
        new { RoleId = RoleId.Create(RoleType.Registered.Id), PermissionId = Permission.EventsRead.Id },
        new { RoleId = RoleId.Create(RoleType.Registered.Id), PermissionId = Permission.ClubsRead.Id },
        new { RoleId = RoleId.Create(RoleType.Registered.Id), PermissionId = Permission.UsersRead.Id },

        // Moderator: content management
        new { RoleId = RoleId.Create(RoleType.Moderator.Id), PermissionId = Permission.EventsRead.Id },
        new { RoleId = RoleId.Create(RoleType.Moderator.Id), PermissionId = Permission.EventsCreate.Id },
        new { RoleId = RoleId.Create(RoleType.Moderator.Id), PermissionId = Permission.EventsUpdate.Id },
        new { RoleId = RoleId.Create(RoleType.Moderator.Id), PermissionId = Permission.ClubsRead.Id },
        new { RoleId = RoleId.Create(RoleType.Moderator.Id), PermissionId = Permission.ClubsCreate.Id },
        new { RoleId = RoleId.Create(RoleType.Moderator.Id), PermissionId = Permission.ClubsUpdate.Id },
        new { RoleId = RoleId.Create(RoleType.Moderator.Id), PermissionId = Permission.UsersRead.Id },
        new { RoleId = RoleId.Create(RoleType.Moderator.Id), PermissionId = Permission.UsersUpdate.Id },

        // Admin: all permissions
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.EventsRead.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.EventsCreate.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.EventsUpdate.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.EventsDelete.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.UsersRead.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.UsersUpdate.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.ClubsRead.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.ClubsCreate.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.ClubsUpdate.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.ClubsDelete.Id },
        new { RoleId = RoleId.Create(RoleType.Admin.Id), PermissionId = Permission.AdminPanel.Id },
    ];
}
