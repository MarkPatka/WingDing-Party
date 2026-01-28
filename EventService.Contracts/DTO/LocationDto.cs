namespace EventService.Contracts.DTO;

public sealed record LocationDto(
    string Address,
    string City,
    string Country);

public sealed record LocationFullDto(
    string Address,
    string City,
    string Country,
    decimal? Latitude,
    decimal? Longitude);