using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.Enumerations;

public sealed class EventStatus : Enumeration
{
    public static readonly EventStatus Draft     = new(1, nameof(Draft),     "Draft");
    public static readonly EventStatus Active    = new(2, nameof(Active),    "Active");
    public static readonly EventStatus Cancelled = new(3, nameof(Cancelled), "Cancelled");
    public static readonly EventStatus Completed = new(4, nameof(Completed), "Finished");
    public static readonly EventStatus Deleted   = new(5, nameof(Deleted), "Deleted");

    private EventStatus(int id, string name, string? description = null)
        : base(id, name, description) { }
}
