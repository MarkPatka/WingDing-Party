using EventService.Application.EventManagement.Common;
using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain.EventAggregate.Entities;
using MediatR;

namespace EventService.Application.EventManagement.Command.CreateEventTypeCommand;

public class CreateEventTypeCommandHandler
    : IRequestHandler<CreateEventTypeCommand, CreateEventTypeResult>
{
    private readonly IEventTypeService _eventTypeService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventTypeCommandHandler(
        IEventTypeService eventTypeService,
        IUnitOfWork unitOfWork)
    {
        _eventTypeService = eventTypeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateEventTypeResult> Handle(
        CreateEventTypeCommand request, CancellationToken cancellationToken)
    {
        var eventTypeExists = await _eventTypeService
            .CheckEventTypeNotExists(request.Name, cancellationToken);

        if (eventTypeExists)
            throw new InvalidOperationException(
                $"EventType by Name: {request.Name} already exists");

        var newEventType = EventType.Create(
            request.Name, request.Description, request.Icon);

        await _eventTypeService.CreateEventTypeAsync(newEventType, cancellationToken);

        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CreateEventTypeResult(newEventType.Id.Value);
    }
}
