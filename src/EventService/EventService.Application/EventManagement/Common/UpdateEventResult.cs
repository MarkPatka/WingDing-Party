namespace EventService.Application.EventManagement.Common;

public record UpdateEventResult(Guid EventId, DateTime UpdatedAt, int StatusId);
