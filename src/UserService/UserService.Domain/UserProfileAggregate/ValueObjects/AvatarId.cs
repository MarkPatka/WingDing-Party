using UserService.Domain.Common.Abstract;

namespace UserService.Domain.UserProfileAggregate.ValueObjects;

public sealed class AvatarId : ValueObject, IEntityId
{
    private AvatarId(Uri value) => Value = value;
    public Uri Value { get; }
    object IEntityId.Value => Value;
    public static AvatarId Create(Uri value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}