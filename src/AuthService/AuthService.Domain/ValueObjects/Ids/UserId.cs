using AuthService.Domain.Common.Abstractions;

namespace AuthService.Domain.ValueObjects.Ids;

public sealed class UserId : ValueObject
{
    public Guid Value { get; }
    public static UserId Create(Guid value) => new(value);
    public static UserId CreateUnique() => new(Guid.NewGuid());

    private UserId(Guid value) => Value = value;
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
