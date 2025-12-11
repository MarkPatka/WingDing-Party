using WingDing_Party.IdentityProvider.Domain.Common.Abstract;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

public sealed class RefreshTokenId : ValueObject
{
    public Guid Value { get; private set; }

    private RefreshTokenId(Guid value)
    {
        Value = value;
    }

    public static RefreshTokenId CreateUnique() => new(Guid.NewGuid());

    public static RefreshTokenId Create(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}