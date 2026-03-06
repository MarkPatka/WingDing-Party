using Mapster;
using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventManagement.Common;
using EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;
using EventService.Contracts.Events;
using EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;

namespace EventService.Api.Mapping;

public class EventMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateEventRequest, CreateEventCommand>();

        config.NewConfig<CreateEventResult, CreateEventResponse>();

        config.NewConfig<GetAllUserEventsRequest, GetAllUserEventsQuery>();

        config.NewConfig<GetAllUserEventsResult, GetAllUserEventsResponse>();

        config.NewConfig<GetEventsByTextAndFiltersRequest, GetEventsByTextAndFiltersQuery>();

        config.NewConfig<GetEventsByTextAndFiltersResult, GetEventsByTextAndFiltersResponse>();
    }
}