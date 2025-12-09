using System.Reflection;

namespace WingDing_Party.AuthenticationService.Domain.Common.Abstract;

public abstract class Enumeration
    : IComparable, IEquatable<Enumeration>
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = null;

    protected Enumeration(int id, string name, string? description = null) =>
        (Id, Name, Description) = (id, name, description);

    public static IEnumerable<T> GetAll<T>()
        where T : Enumeration => typeof(T)
        .GetFields(
            BindingFlags.Public |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly)
        .Select(x => x.GetValue(null))
        .Cast<T>();

    public static T GetFromId<T>(int id)
        where T : Enumeration => Parse<T, int>(id, i => i.Id == id)
        ;

    public static T Parse<T, K>(K value, Func<T, bool> predicate)
        where T : Enumeration => GetAll<T>().FirstOrDefault(predicate)
            ?? throw new ApplicationException($"{value} is not valid item for the type: {typeof(T)}");

    public override string ToString() => Name;
    public override int GetHashCode() => Id.GetHashCode();
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration other || obj is null)
            return false;

        var typeMatches = GetType().Equals(other.GetType());
        var valueMatches = Id.Equals(other.Id);

        return typeMatches && valueMatches;
    }
    public int CompareTo(object? other)
    {
        if (other is null) return 0;

        return Id.CompareTo(((Enumeration)other).Id);
    }
    public bool Equals(Enumeration? other)
    {
        if (other is null) return false;

        var typeMatches = GetType().Equals(other.GetType());
        var valueMatches = Id.Equals(other.Id);

        return typeMatches && valueMatches;
    }
}