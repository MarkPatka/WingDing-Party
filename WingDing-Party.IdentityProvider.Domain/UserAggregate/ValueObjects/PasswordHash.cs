using WingDing_Party.IdentityProvider.Domain.Common.Abstract;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    public string Value { get; }

    private PasswordHash(string value) => Value = value;

    public static PasswordHash Create(string hashedPassword) => new(hashedPassword);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
