using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Services;

public interface IEventTypeService
{
    Task<EventType?> GetEventTypeByIdAsync(
        EventTypeId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventType>> GetAllEventTypesAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<EventType> CreateEventTypeAsync(
        EventType eventType,
        CancellationToken cancellationToken = default);

    Task<bool> EventTypeIdExistsAsync(
        EventTypeId id, CancellationToken cancellationToken = default);

    Task<EventType?> GetDefaultEventTypeAsync(
        CancellationToken cancellationToken = default);

    public Task<bool> CheckEventTypeNotExists(
        string name, CancellationToken cancellationToken);
}
