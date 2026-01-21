using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.ValueObjects;

public sealed class EventId : ValueObject
{
    public Guid Value { get; }

    private EventId(Guid value) => Value = value;

    public static EventId Create(Guid value) => new(value);
    public static EventId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}


