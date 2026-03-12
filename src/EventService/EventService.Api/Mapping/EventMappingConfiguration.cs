using Mapster;
using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventManagement.Common;
using EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;
using EventService.Contracts.Events;
using EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;
using EventService.Domain.EventAggregate.ValueObjects;
using EventService.Contracts.DTO;
using EventService.Application.EventManagement.Command.UpdateEventCommand;
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
                MapLocation(src.Location)
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
                MapLocation(src.Location),
                src.StartDate,
                src.EndDate,
                src.MaxParticipants
                ));

        config.NewConfig<UpdateEventResult, UpdateEventResponse>();
    }
    private static Location? MapLocation(LocationFullDto? dto) =>
            dto == null ? null : Location.Create(
                dto.Address,
                dto.City,
                dto.Country,
                dto.Latitude,
                dto.Longitude
            );
}