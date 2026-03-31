using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventService.Infrastructure.Persistence.DatabaseModelsConfigurations;

public class EventTypeConfigurations : IEntityTypeConfiguration<EventType>
{
    public void Configure(EntityTypeBuilder<EventType> builder)
    {
        builder.ToTable("EventTypes");

        builder.HasKey(et => et.Id);

        builder.Property(et => et.Id)
            .HasColumnName("EventTypeId")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => EventTypeId.Create(value));

        builder.Property(et => et.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(et => et.Description)
            .HasMaxLength(500);

        builder.Property(et => et.Icon)
            .HasMaxLength(200);

        builder.Property(et => et.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(et => et.Name)
            .IsUnique()
            .HasDatabaseName("IX_EventTypes_Name");

        builder.HasIndex(et => et.IsDefault)
            .IsUnique()
            .HasFilter("\"IsDefault\" = true");
    }
}
