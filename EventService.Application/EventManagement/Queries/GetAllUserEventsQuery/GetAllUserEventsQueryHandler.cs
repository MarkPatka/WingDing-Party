using EventService.Application.EventManagement.Common;
using MediatR;
using EventService.Application.Persistence;

namespace EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;

public class GetAllUserEventsQueryHandler
    : IRequestHandler<GetAllUserEventsQuery, GetAllUserEventsResult>
{
    private readonly IEventRepository _repository;

    public GetAllUserEventsQueryHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllUserEventsResult> Handle(GetAllUserEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _repository.GetByFilter(x => x.OrganizerId.Value.ToString() == request.userId);

        return await Task.FromResult(new GetAllUserEventsResult(events));
    }
}
