using WingDing_Party.IdentityProvider.Domain.Common.Abstract;
using WingDing_Party.IdentityProvider.Domain.Common.ResultPattern;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; private set; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        // validation
        var result = new PhoneNumber(phoneNumber);

        return Result<PhoneNumber>.Success(result);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
