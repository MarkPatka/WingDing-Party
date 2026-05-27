using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventManagement.Command.CreateEventTypeCommand;
using EventService.Application.EventManagement.Command.DeleteEventCommand;
using EventService.Application.EventManagement.Command.RegisterParticipant;
using EventService.Application.EventManagement.Command.UpdateEventCommand;
using EventService.Application.EventManagement.Common;
using EventService.Application.EventManagement.Queries.GetAllEventTypesQuery;
using EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;
using EventService.Application.EventManagement.Queries.GetEventByIdQuery;
using EventService.Application.EventManagement.Queries.GetEventParticipantsQuery;
using EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;
using EventService.Application.EventManagement.Queries.GetTopRatedEventsByStartDateWithLimitQuery;
using EventService.Contracts.DTO;
using EventService.Contracts.Events.Requests;
using EventService.Contracts.Events.Responses;
using EventService.Domain.EventAggregate.ValueObjects;
using Mapster;
using EventId = EventService.Domain.EventAggregate.ValueObjects.EventId;

namespace EventService.Api.Mapping;

public class EventMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LocationFullDto, Location>()
            .ConstructUsing(src => Location.Create(
                src.Address,
                src.City,
                src.Country,
                src.Latitude,
                src.Longitude
            ));

        config.NewConfig<CreateEventRequest, CreateEventCommand>()
            .ConstructUsing(src => new CreateEventCommand(
                src.Title,
                src.EventTypeId,
                src.StartDate,
                src.EndDate,
                src.MaxParticipants,
                src.OrganizerId,
                src.Description,
                src.Location.Adapt<Location>()
            ));

        config.NewConfig<CreateEventResult, CreateEventResponse>();

        config.NewConfig<GetAllUserEventsRequest, GetAllUserEventsQuery>(); 

        config.NewConfig<GetAllUserEventsResult, GetAllUserEventsResponse>();

        config.NewConfig<GetEventsByTextAndFiltersRequest, GetEventsByTextAndFiltersQuery>();

        config.NewConfig<GetEventsByTextAndFiltersResult, GetEventsByTextAndFiltersResponse>();

        config.NewConfig<UpdateEventRequest, UpdateEventCommand>()
            .ConstructUsing(src => new UpdateEventCommand(
                EventId.Create(src.EventId),
                src.Title,
                src.Description,
                src.Location.Adapt<Location>(),
                src.StartDate,
                src.EndDate,
                src.MaxParticipants
                ));

        config.NewConfig<UpdateEventResult, UpdateEventResponse>();

        config.NewConfig<DeleteEventRequest, DeleteEventCommand>()
            .ConstructUsing(src => new DeleteEventCommand(EventId.Create(src.EventId)));

        config.NewConfig<DeleteEventResult, DeleteEventResponse>();

        config.NewConfig<RegisterParticipantRequest, RegisterParticipantCommand>()
            .ConstructUsing(src => new RegisterParticipantCommand(
                EventId.Create(src.EventId),
                UserId.Create(src.UserId),
                src.UserName));

        config.NewConfig<RegisterParticipantResult, RegisterParticipantResponse>();

        config.NewConfig<GetEventByIdRequest, GetEventByIdQuery>();

        config.NewConfig<GetEventByIdResult, GetEventByIdResponse>();

        config.NewConfig<GetTopRatedEventsByStartDateWithLimitRequest, GetTopRatedEventsByStartDateWithLimitQuery>();

        config.NewConfig<GetTopRatedEventsByStartDateWithLimitResult, GetTopRatedEventsByStartDateWithLimitResponse>();

        config.NewConfig<CreateEventTypeRequest, CreateEventTypeCommand>();

        config.NewConfig<CreateEventTypeResult, CreateEventTypeResponse>();

        config.NewConfig<GetAllEventTypesRequest, GetAllEventTypesQuery>();

        config.NewConfig<GetAllEventTypesResult, GetAllEventTypesResponse>();

        config.NewConfig<GetEventParticipantsRequest, GetEventParticipantsQuery>();

        config.NewConfig<GetEventParticipantsResult, GetEventParticipantsResponse>();
    }
}