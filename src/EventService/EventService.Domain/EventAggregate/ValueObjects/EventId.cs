using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.ValueObjects;

public sealed class EventId : ValueObject, IEntityId
{
    public Guid Value { get; }

    object IEntityId.Value => Value;

    private EventId(Guid value) => Value = value;

    public static EventId Create(Guid value) => new(value);
    public static EventId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}


