using EventService.Domain.EventAggregate.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventService.Infrastructure.Persistence.DatabaseModelsConfigurations;

public class EventStatusConfiguration : IEntityTypeConfiguration<EventStatus>
{
    public void Configure(EntityTypeBuilder<EventStatus> builder)
    {
        builder.ToTable("EventStatuses");

        builder.HasKey(es => es.Id);

        builder.Property(es => es.Id)
            .HasColumnName("EventStatusId")
            .ValueGeneratedNever();

        builder.Property(es => es.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(es => es.Description)
            .HasMaxLength(100);

        builder.HasData(EventStatus.List);
    }
}
