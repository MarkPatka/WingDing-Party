using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.Entities;

public sealed class EventType : Entity<EventTypeId>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public string? Icon { get; set; } = null;

    private EventType(Guid id,
        string name,
        string? description,
        string? icon)
        : base(EventTypeId.Create(id))
    {
        Name = name;
        Description = description;
        Icon = icon;
    }

    private EventType(
        string name,
        string? description,
        string? icon)
        : base(EventTypeId.CreateUnique())
    {
        Name = name;
        Description = description;
        Icon = icon;
    }

    public static EventType CreateNew(
        Guid id, string name, string? description, string? icon)
            => new(id, name, description, icon);

    public static EventType Create(
        string name, string? description, string? icon)
            => new(name, description, icon);
}
