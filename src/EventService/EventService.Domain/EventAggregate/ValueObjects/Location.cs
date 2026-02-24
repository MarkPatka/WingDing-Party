using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.ValueObjects;

public sealed class Location : ValueObject
{
    public string Address { get; }
    public string City { get; }
    public string Country { get; }

    public decimal? Latitude { get; }
    public decimal? Longitude { get; }

    private Location(string address, string city, string country, decimal? latitude = null, decimal? longitude = null)
    {
        Address = address;
        City = city;
        Country = country;
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Location Create(string address, string city, string country, decimal? latitude = null, decimal? longitude = null)
        => new(address, city, country, latitude, longitude);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
        yield return City;
        yield return Country;
    }
}
