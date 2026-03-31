using EventService.Domain;
using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.Enumerations;
using EventService.Domain.EventAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventService.Infrastructure.Persistence.DatabaseModelsConfigurations;

public class EventConfigurations : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        ConfigureEventsTable(builder);
        ConfigureLocation(builder);
        ConfigureParticipantTable(builder);
    }

    private static void ConfigureEventsTable(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("EventId")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => EventId.Create(value));

        builder.Property(e => e.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        // OrganizerId (ссылка на User из UserService)
        builder.Property(e => e.OrganizerId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.Property(e => e.OrganizerName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property<Guid>("EventTypeId")
            .HasColumnName("EventTypeId")
            .IsRequired();

        builder.HasOne(e => e.EventType)
            .WithMany()
            .HasForeignKey("EventTypeId")
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Events_EventTypes_EventTypeId");

        builder.Property(e => e.Status)
            .HasConversion(
                status => status.Name,
                value => Enumeration.GetFromName<EventStatus>(value))
            .HasMaxLength(50)
            .IsRequired();

        // Данные отзывов (обновляются через поглощение событий ReviewService)
        builder.Property(e => e.ReviewsCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(e => e.AverageRating)
            .HasPrecision(3, 2)
            .IsRequired(false);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.Property(e => e.MaxParticipants)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Автоматически bytea(8000) в PostgreSQL
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();

        builder.HasIndex("EventTypeId")
            .HasDatabaseName("IX_Events_EventTypeId");

        builder.HasIndex(e => e.OrganizerId)
            .HasDatabaseName("IX_Events_OrganizerId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Events_Status");

        builder.HasIndex(e => e.StartDate)
            .HasDatabaseName("IX_Events_StartDate");

        builder.HasIndex(e => new { e.Status, e.StartDate })
            .HasDatabaseName("IX_Events_Status_StartDate");

        builder.HasIndex(e => new { e.OrganizerId, e.Status })
            .HasDatabaseName("IX_Events_OrganizerId_Status");
    }
    private static void ConfigureLocation(EntityTypeBuilder<Event> builder)
    {
        builder.OwnsOne(e => e.Location, lb =>
        {
            lb.Property(l => l.Address)
                .HasColumnName("LocationAddress")
                .HasMaxLength(200);

            lb.Property(l => l.City)
                .HasColumnName("LocationCity")
                .HasMaxLength(100);

            lb.Property(l => l.Country)
                .HasColumnName("LocationCountry")
                .HasMaxLength(100);

            lb.Property(l => l.Latitude)
                .HasColumnName("LocationLatitude")
                .HasPrecision(10, 7);

            lb.Property(l => l.Longitude)
                .HasColumnName("LocationLongitude")
                .HasPrecision(10, 7);
        });
    }
    private static void ConfigureParticipantTable(EntityTypeBuilder<Event> builder)
    {
        builder.OwnsMany(e => e.Participants, pb =>
        {
            pb.ToTable("Participants");

            pb.HasKey(nameof(Participant.Id));

            // OwnsMany по умолчанию использует DeleteBehavior.Cascade
            // Прямая установка через Metadata.DeleteBehavior = DeleteBehavior.Cascade
            pb.WithOwner()
                .HasForeignKey("EventId");

            pb.Property(p => p.Id)
                .HasColumnName("ParticipantId")
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => ParticipantId.Create(value));

            pb.Property(p => p.EventId)
                .HasConversion(
                    id => id.Value,
                    value => EventId.Create(value));

            pb.Property(p => p.UserId)
                .HasConversion(
                    id => id.Value,
                    value => UserId.Create(value));

            pb.Property(p => p.UserName)
                .HasMaxLength(100)
                .IsRequired();

            pb.Property(p => p.RegisteredAt)
                .IsRequired();

            pb.Property(p => p.Status)
                .HasConversion(
                    status => status.Name,
                    value => Enumeration.GetFromName<ParticipantStatus>(value))
                .HasMaxLength(50)
                .IsRequired();

            // один юзер не может зарегистрироваться дважды на одно мероприятие
            pb.HasIndex(p => new { p.EventId, p.UserId })
                .IsUnique()
                .HasDatabaseName("IX_Participants_EventId_UserId");

            pb.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_Participants_UserId");

            pb.HasIndex(p => p.Status)
                .HasDatabaseName("IX_Participants_Status");
        });

        builder.Navigation(d => d.Participants)
            .Metadata.SetField("_participants");

        builder.Metadata
            .FindNavigation(nameof(Event.Participants))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
