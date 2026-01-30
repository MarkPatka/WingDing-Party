using EventService.Domain.Common.Abstract;
using Microsoft.Extensions.Logging;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.Entities;

public sealed class Review : Entity<ReviewId>
{
    public ValueObjects.EventId EventId { get; set; } = null!;
    public string ReviewerName { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    private Review() { }

    public Review(ReviewId id) : base(id) { }
}