using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.ValueObjects;

public sealed class OrganizerId : ValueObject
{
    public Guid Value { get; }

    private OrganizerId(Guid value) => Value = value;

    public static OrganizerId Create(Guid value) => new(value);
    public static OrganizerId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
