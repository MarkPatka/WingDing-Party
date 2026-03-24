using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Infrastructure.Persistence.DatabaseModelsConfigurations;

public class UserProfileConfigurations : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        ConfigureUserProfileTable(builder);
    }

    private void ConfigureUserProfileTable(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value)).IsRequired();

        builder.Property(e => e.DisplayName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Bio).HasMaxLength(500);

        builder.Property(e => e.AvatarUri)
            .HasMaxLength(256)
            .HasConversion(
                uri => uri == null ? null : uri.ToString(),
                value => value == null ? null : new Uri(value));

        builder.Property(e => e.Interests).HasColumnType("text[]");
        builder.HasIndex(e => e.Interests).HasMethod("gin");

        builder.Property(e => e.BirthDate).HasColumnType("timestamp with time zone");
        builder.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(e => e.Id);
    }
}