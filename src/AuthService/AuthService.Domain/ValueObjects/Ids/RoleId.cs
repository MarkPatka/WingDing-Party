using AuthService.Domain.Common.Abstractions;

namespace AuthService.Domain.ValueObjects.Ids;

public sealed class RoleId : ValueObject
{
    public int Value { get; }

    private RoleId(int value) => Value = value;

    public static RoleId Create(int value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
