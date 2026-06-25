using ClubService.Domain.Common.Abstract;

namespace ClubService.Domain.ClubAggregate.ValueObjects;

public sealed class OwnerId : ValueObject, IEntityId
{
    private OwnerId(Guid value) => Value = value;
    public Guid Value { get; }
    object IEntityId.Value => Value;
    public static OwnerId Create(Guid value) => new(value);
    public static OwnerId CreateUnique() => new(Guid.NewGuid());
    public override string ToString() => $"{Value}";

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}


