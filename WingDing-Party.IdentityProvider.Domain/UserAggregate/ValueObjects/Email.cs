using WingDing_Party.IdentityProvider.Domain.Common.Abstract;
using WingDing_Party.IdentityProvider.Domain.Common.ResultPattern;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<Email>.Failure("Email cannot be empty");

        if (!IsValidEmail(email))
            return Result<Email>.Failure("Invalid email format");

        return Result<Email>.Success(new Email(email.ToLowerInvariant()));
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}