namespace EventService.Application.EventManagement.Common;

public record CreateEventResult(
        Guid EventId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int StatusId
    );
