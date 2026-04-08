namespace EventService.Contracts.DTO;

public sealed record EventTypeDto(
    string Id,
    string Name,
    string? Description,
    string? Icon);
