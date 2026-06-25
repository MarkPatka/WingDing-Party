using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.Persistence.DatabaseModelsConfigurations;

public class ClubConfigurations : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        ConfigureClubTable(builder);
    }

    private void ConfigureClubTable(EntityTypeBuilder<Club> builder)
    {
        builder.ToTable("Clubs");
        builder.Property(x => x.Id).HasConversion(
                id => id.Value,
                value => ClubId.Create(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2048);

        builder.OwnsMany(c => c.ClubMembers, mb =>
        {
            mb.ToTable("ClubMembers");
            mb.Property(x => x.Id)
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn();
            mb.Property(x => x.JoinedAt).IsRequired();
            mb.Property<ClubId>("ClubId")
                .HasConversion(
                    id => id.Value,
                    value => ClubId.Create(value))
                .IsRequired();

            mb.Property<UserId>("UserId")
                .HasConversion(
                    id => id.Value,
                    value => UserId.Create(value))
                .IsRequired();

            mb.WithOwner().HasForeignKey("ClubId");
            mb.HasKey("ClubId", "UserId");
        });

        builder.Property(x => x.Owner).HasConversion(
            owner => owner.Value,
            value => OwnerId.Create(value)).IsRequired();

        builder.Property(e => e.Interests).HasColumnType("text[]");
        builder.Property(e => e.IsPublic).HasColumnType("bool");
        builder.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnType("timestamp with time zone");

        builder.Navigation(d => d.ClubMembers).Metadata.SetField("_members");
        builder.Navigation(d => d.ClubMembers).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}