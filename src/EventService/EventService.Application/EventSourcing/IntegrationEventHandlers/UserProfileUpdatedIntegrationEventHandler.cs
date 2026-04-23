using EventService.Application.EventSourcing.IntegrationEvents.Incoming;
using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain.EventAggregate.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EventService.Application.EventSourcing.IntegrationEventHandlers;

public sealed class UserProfileUpdatedIntegrationEventHandler
    : IIntegrationEventHandler<UserProfileUpdatedIntegrationEvent>
{
    private readonly IEventService _eventService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserProfileUpdatedIntegrationEventHandler> _logger;

    public UserProfileUpdatedIntegrationEventHandler(
        IEventService eventService,
        IUnitOfWork unitOfWork,
        ILogger<UserProfileUpdatedIntegrationEventHandler> logger)
    {
        _eventService = eventService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(
        UserProfileUpdatedIntegrationEvent integrationEvent, 
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(integrationEvent.UserId);
        var newName = integrationEvent.DisplayName;

        _logger.LogInformation(
            "Synchronizing display name for UserId={UserId} -> '{NewName}'",
            userId.Value, newName);

        // 1. Events where user is organizer
        var organized = await _eventService.GetEventsByOrganizerIdAsync(
            userId, pageNumber: 1, pageSize: byte.MaxValue, cancellationToken);

        foreach (var ev in organized)
            ev.UpdateOrganizerName(newName);

        // 2. Events where user is participant
        var participated = await _eventService.GetEventsByParticipantAsync(
            userId, cancellationToken);

        foreach (var ev in participated)
            ev.UpdateParticipantName(userId, newName);

        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation(
            "Display name synchronized: {OrganizedCount} organized + " +
            "{ParticipatedCount} participated",
            organized.Count, participated.Count);
    }
}
