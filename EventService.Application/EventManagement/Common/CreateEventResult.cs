using EventService.Domain.EventAggregate.Enumerations;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.EventManagement.Common;

public record CreateEventResult(
        Guid EventId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int StatusId
    );
