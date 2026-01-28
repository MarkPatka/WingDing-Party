using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.ValueObjects;

public sealed class ParticipantId : ValueObject, IEntityId
{
    public Guid Value { get; }

    object IEntityId.Value => Value;

    private ParticipantId(Guid value) => Value = value;

    public static ParticipantId Create(Guid value) => new(value);
    public static ParticipantId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}