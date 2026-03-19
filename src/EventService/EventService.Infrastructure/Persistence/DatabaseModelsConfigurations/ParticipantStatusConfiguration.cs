using EventService.Domain.EventAggregate.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventService.Infrastructure.Persistence.DatabaseModelsConfigurations;

public class ParticipantStatusConfiguration : IEntityTypeConfiguration<ParticipantStatus>
{
    public void Configure(EntityTypeBuilder<ParticipantStatus> builder)
    {
        builder.ToTable("ParticipantStatuses");

        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Id)
            .HasColumnName("ParticipantStatusId")
            .ValueGeneratedNever();

        builder.Property(ps => ps.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ps => ps.Description)
            .HasMaxLength(100);

        builder.HasData(ParticipantStatus.List);
    }
}
