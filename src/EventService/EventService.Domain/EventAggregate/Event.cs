using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.DomainEvents;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.Enumerations;
using EventService.Domain.EventAggregate.ValueObjects;
using EventId = EventService.Domain.EventAggregate.ValueObjects.EventId;

namespace EventService.Domain;

public sealed class Event : AggregateRoot<EventId>
{
    //private readonly List<Review> _reviews = [];
    private readonly List<Participant> _participants = [];

    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();
    //public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    public string Title         { get; private set; } = string.Empty;
    public string Description   { get; private set; } = string.Empty;
    public EventType EventType  { get; private set; } = null!;
    public Location Location    { get; private set; } = null!;
    public DateTime StartDate   { get; private set; }
    public DateTime EndDate     { get; private set; }
    public int MaxParticipants  { get; private set; }
    public EventStatus Status   { get; private set; } = EventStatus.Draft;

    public UserId OrganizerId  { get; private set; } = null!;
    public string OrganizerName     { get; private set; } = string.Empty;

    public DateTime CreatedAt   { get; private set; }
    public DateTime? UpdatedAt  { get; private set; }

    public int ReviewsCount         { get; private set; }
    public decimal? AverageRating   { get; private set; }

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
        UserId organizerId,
        DateTime createdAt,
        DateTime updatedAt,
        int reviewsCount = 0,
        decimal? averageRating = null
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
        ReviewsCount = reviewsCount;
        AverageRating = averageRating;
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
        UserId organizerId,
        DateTime createdAt,
        DateTime updatedAt,
        int reviewsCount = 0,
        decimal? averageRating = null)
    {
        if (startDate > endDate)
            throw new InvalidOperationException("End date must be greater start date");

        if (maxParticipants <= 0)
            throw new InvalidOperationException("Max participants must be greater 0");

        if (string.IsNullOrEmpty(title))
            throw new InvalidOperationException("Title must not be empty");

        var @event = new Event(
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
            updatedAt,
            reviewsCount,
            averageRating);

        @event.AddDomainEvent(new EventCreated(@event.Id, DateTime.UtcNow));
        return @event;
    }

    public void Update()
    {
        if (Status != EventStatus.Draft && Status != EventStatus.Active)
            throw new InvalidOperationException("Only draft and active events can be updated");

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EventUpdated(Id, DateTime.UtcNow));
    }

    public void Publish()
    {
        if (Status != EventStatus.Draft)
            throw new InvalidOperationException("Only draft events can be published");

        Status = EventStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EventPublished(Id, DateTime.UtcNow));
    }

    public void Cancell()
    {
        if (Status != EventStatus.Active)
            throw new InvalidOperationException("Only active events can be cancelled");

        Status = EventStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EventCancelled(Id, DateTime.UtcNow));
    }

    public void Delete()
    {
        if (Status != EventStatus.Draft && Status != EventStatus.Cancelled)
            throw new InvalidOperationException(
                "Only Draft/Cancelled events can be deleted; Active requires cancel first");

        Status = EventStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EventDeleted(Id, DateTime.UtcNow));
    }

    public Participant RegisterParticipant(UserId userId, string name)
    {
        if (Status != EventStatus.Active)
            throw new InvalidOperationException("Only active events accept registrations");

        if (_participants.Count >= MaxParticipants)
            throw new InvalidOperationException("Event has reached maximum participants");

        if (_participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException("User is already registered in the event");

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Participant name is required");

        var participant = Participant.Create(Id, userId, name, DateTime.UtcNow);

        _participants.Add(participant);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ParticipantRegistered(Id, participant.Id, participant.RegisteredAt));

        return participant;
    }

    public void UpdateReviewStats(int reviewsCount, decimal averageRating)
    {
        if (reviewsCount < 0)
            throw new ArgumentException("Reviews count cannot be negative", nameof(reviewsCount));

        if (averageRating < 0 || averageRating > 5)
            throw new ArgumentException("Average rating must be between 0 and 5", nameof(averageRating));

        ReviewsCount = reviewsCount;
        AverageRating = reviewsCount > 0 ? averageRating : null;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetRatingDisplay() => AverageRating.HasValue
        ? $"{AverageRating.Value:F2}"
        : "No rating yet";
}