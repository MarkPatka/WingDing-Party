using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.Entities;

public sealed class EventType : Entity<EventTypeId>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public string? Icon { get; set; } = null;
    public bool IsDefault { get; private set; }

    private EventType(Guid id,
        string name,
        string? description,
        string? icon,
        bool isDefault = false)
        : base(EventTypeId.Create(id))
    {
        Name = name;
        Description = description;
        Icon = icon;
        IsDefault = isDefault;
    }

    private EventType(
        string name,
        string? description,
        string? icon,
        bool isDefault = false)
        : base(EventTypeId.CreateUnique())
    {
        Name = name;
        Description = description;
        Icon = icon;
        IsDefault = isDefault;
    }

    public static EventType CreateNew(
        Guid id, string name, string? description, string? icon, bool isDefault = false)
            => new(id, name, description, icon, isDefault);

    public static EventType Create(
        string name, string? description, string? icon, bool isDefault = false)
            => new(name, description, icon, isDefault);
}
