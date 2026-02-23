using EventService.Domain;
using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.Enumerations;
using EventService.Domain.EventAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventService.Infrastructure.Persistence.DatabaseModelsConfigurations;

public class EventConfigurations : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        ConfigureEventsTable(builder);
        //ConfigureReviewsTable(builder);
        // ConfigureParticipantsTable(builder);
    }
    /*
    private void ConfigureReviewsTable(EntityTypeBuilder<Event> builder)
    {
        builder.OwnsMany(r => r.Reviews, rb =>
        {
            rb.ToTable("Reviews");

            // IF THE KEY CONSIST OF SEVERAL COLUMNS USE:
            // rb.HasKey("IdFirst", "IdSecond");
            // or rb.HasKey(new[] { x.Id_1, x.Id_2 });

            rb.HasKey(nameof(Review.Id), "ReviewId");

            rb.WithOwner().HasForeignKey("EventId");

            rb.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => ReviewId.Create(value));

            rb.Property(x => x.CreatedAt);
            rb.Property(x => x.Comment)
                .HasMaxLength(200);

            rb.Property(x => x.Rating); // restrict to be 1 to 100 in domain
            rb.Property(x => x.ReviewerName);
        });

        builder.Metadata
            .FindNavigation(nameof(Event.Reviews))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
    */

    // COMPLETE PARTICIPANT TABLE CONFIGURATION 

    private void ConfigureEventsTable(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);
        // MAPPING ID VALUE OBJECT
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,                  // ЛОГИКА ЕСЛИ ПОЛУЧАЕМ НА ВХОД EventId, а вернуть должны Guid
                value => EventId.Create(value)); // ЛОГИКА ЕСЛИ ПОЛУЧАЕМ НА ВХОД Guid, а вернуть должны EventId

        builder.Property(e => e.Title)
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        // MAPPING ENUMERATIONS
        builder.Property(x => x.Status)
            .HasConversion(
            new ValueConverter<EventStatus, string>(
                type => type.Name,
                value => Enumeration.GetFromName<EventStatus>(value)));

        // CHILDREN ENTITY WITH IDENTITY
        builder.OwnsOne(e => e.EventType, et =>
        {
            et.HasKey(x => x.Id);

            builder.Property(x => x.EventType.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => EventTypeId.Create(value));

            et.Property(x => x.Name)
                .HasMaxLength(100);

            et.Property(x => x.Description);
            et.Property(x => x.Icon);
        });

        builder.Property(e => e.StartDate);
        builder.Property(e => e.EndDate);
    }
}
