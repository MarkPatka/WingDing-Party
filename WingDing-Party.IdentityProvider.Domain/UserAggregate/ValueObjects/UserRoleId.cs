using WingDing_Party.IdentityProvider.Domain.Common.Abstract;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

public sealed class UserRoleId : ValueObject
{
    public Guid Value { get; private set; }

    private UserRoleId(Guid value)
    {
        Value = value;
    }

    public static UserRoleId CreateUnique() => new(Guid.NewGuid());

    public static UserRoleId Create(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
