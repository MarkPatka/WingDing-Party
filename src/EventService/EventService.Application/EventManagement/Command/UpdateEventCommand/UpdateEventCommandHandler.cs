using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Command.UpdateEventCommand;

public class UpdateEventCommandHandler
    : IRequestHandler<UpdateEventCommand, UpdateEventResult>
{
    public Task<UpdateEventResult> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
