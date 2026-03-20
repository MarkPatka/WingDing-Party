using UserService.Domain.Common.Abstract;

namespace UserService.Domain.UserProfileAggregate.ValueObjects;

public sealed class UserId : ValueObject, IEntityId
{
    private UserId(Guid value) => Value = value;
    public Guid Value { get; }
    object IEntityId.Value => Value;
    public static UserId Create(Guid value) => new(value);
    public static UserId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}


