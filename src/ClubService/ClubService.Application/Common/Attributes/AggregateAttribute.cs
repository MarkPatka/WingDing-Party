namespace ClubService.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AggregateAttribute : Attribute
{
    public string Name { get; }

    public AggregateAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}