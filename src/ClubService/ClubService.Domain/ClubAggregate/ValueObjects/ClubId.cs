using ClubService.Domain.Common.Abstract;

namespace ClubService.Domain.ClubAggregate.ValueObjects;

public sealed class ClubId : ValueObject, IEntityId
{
    public Guid Value { get; }
    object IEntityId.Value => Value;
    private ClubId(Guid value) => Value = value;
    public static ClubId Create(Guid value) => new(value);
    public static ClubId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}


