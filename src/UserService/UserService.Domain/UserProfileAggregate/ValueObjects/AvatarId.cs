using UserService.Domain.Common.Abstract;

namespace UserService.Domain.UserProfileAggregate.ValueObjects;

public sealed class AvatarId : ValueObject, IEntityId
{
    private AvatarId(Guid value) => Value = value;
    public Guid Value { get; }
    object IEntityId.Value => Value;
    public static AvatarId Create(Guid value) => new(value);
    public static AvatarId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}