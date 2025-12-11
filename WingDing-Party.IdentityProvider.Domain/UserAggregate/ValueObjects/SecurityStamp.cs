using WingDing_Party.IdentityProvider.Domain.Common.Abstract;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

public sealed class SecurityStamp : ValueObject
{
    public string Value { get; }

    private SecurityStamp(string value) => Value = value;

    public static SecurityStamp Generate() => new(Guid.NewGuid().ToString());
    public static SecurityStamp Create(string value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}