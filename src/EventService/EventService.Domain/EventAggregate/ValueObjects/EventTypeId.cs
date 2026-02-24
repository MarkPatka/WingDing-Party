using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.ValueObjects;

public sealed class EventTypeId : ValueObject, IEntityId
{
    public Guid Value { get; }

    object IEntityId.Value => Value;

    private EventTypeId(Guid value) => Value = value;

    public static EventTypeId Create(Guid value) => new(value);
    public static EventTypeId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}