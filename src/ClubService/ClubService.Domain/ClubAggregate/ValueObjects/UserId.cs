using ClubService.Domain.Common.Abstract;

namespace ClubService.Domain.ClubAggregate.ValueObjects;

public sealed class UserId : ValueObject, IEntityId
{
    public Guid Value { get; }
    object IEntityId.Value => Value;
    private UserId(Guid value) => Value = value;
    public static UserId Create(Guid value) => new(value);
    public static UserId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}


