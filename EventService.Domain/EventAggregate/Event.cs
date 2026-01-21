using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.Enumerations;
using EventService.Domain.EventAggregate.ValueObjects;
using EventId = EventService.Domain.EventAggregate.ValueObjects.EventId;

namespace EventService.Domain;

public sealed class Event : AggregateRoot<EventId>
{
    private readonly List<Review> _reviews = [];
    private readonly List<Participant> _participants = [];

    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public EventType EventType { get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int MaxParticipants { get; private set; }
    public EventStatus Status { get; private set; } = EventStatus.Draft;
    public OrganizerId OrganizerId { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Event() { }

    private Event(
        EventId id,
        string title,
        string description,
        EventType eventType,
        Location location,
        DateTime startDate,
        DateTime endDate,
        int maxParticipants,
        OrganizerId organizerId,
        DateTime createdAt,
        DateTime updatedAt
        )
    : base(id)
    {
        Title = title;
        Description = description;
        EventType = eventType;
        Location = location;
        StartDate = startDate;
        EndDate = endDate;
        MaxParticipants = maxParticipants;
        OrganizerId = organizerId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Event Create(
        string title,
        string description,
        EventType eventType,
        Location location,
        DateTime startDate,
        DateTime endDate,
        int maxParticipants,
        OrganizerId organizerId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Event(
            EventId.CreateUnique(),
            title,
            description,
            eventType,
            location,
            startDate,
            endDate,
            maxParticipants,
            organizerId,
            createdAt,
            updatedAt);
    }
}