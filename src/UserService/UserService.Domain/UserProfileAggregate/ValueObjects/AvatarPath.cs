using UserService.Domain.Common.Abstract;

namespace UserService.Domain.UserProfileAggregate.ValueObjects;

public sealed class AvatarPath : ValueObject, IEntityId
{
    private AvatarPath(Uri value) => Value = value;
    public Uri Value { get; }
    object IEntityId.Value => Value;
    public static AvatarPath Create(Uri value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}