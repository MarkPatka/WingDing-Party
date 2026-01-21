using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.ValueObjects;

public sealed class EventTypeId : ValueObject
{
    public Guid Value { get; }

    private EventTypeId(Guid value) => Value = value;

    public static EventTypeId Create(Guid value) => new(value);
    public static EventTypeId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}