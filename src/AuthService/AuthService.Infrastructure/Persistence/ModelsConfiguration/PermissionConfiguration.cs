using AuthService.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.ModelsConfiguration;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.HasData(
            Permission.EventsRead,
            Permission.EventsCreate,
            Permission.EventsUpdate,
            Permission.EventsDelete,
            Permission.UsersRead,
            Permission.UsersUpdate,
            Permission.ClubsRead,
            Permission.ClubsCreate,
            Permission.ClubsUpdate,
            Permission.ClubsDelete,
            Permission.AdminPanel);
    }
}
