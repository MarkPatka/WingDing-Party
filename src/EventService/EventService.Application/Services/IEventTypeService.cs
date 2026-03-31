using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Services;

public interface IEventTypeService
{
    Task<EventType?> GetEventTypeByIdAsync(
        EventTypeId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventType>> GetAllEventTypesAsync(
        CancellationToken cancellationToken = default);

    Task<EventTypeId> CreateEventTypeAsync(
        string name,
        string? description,
        string? IconUrl = null,
        CancellationToken cancellationToken = default);

    Task<bool> EventTypeIdExistsAsync(
        EventTypeId id, CancellationToken cancellationToken = default);

    Task<EventType?> GetDefaultEventTypeAsync(
        CancellationToken cancellationToken = default);
}
