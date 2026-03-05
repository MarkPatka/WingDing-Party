using Mapster;
using EventService.Application.EventManagement.Command.CreateEventCommand;
using EventService.Application.EventManagement.Common;
using EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;
using EventService.Contracts.Events;
using EventService.Domain.EventAggregate.ValueObjects;
using EventService.Contracts.DTO;

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
    }

    private static Location MapLocation(LocationFullDto dto) =>
        Location.Create(
            dto.Address,
            dto.City,
            dto.Country,
            dto.Latitude,
            dto.Longitude
        );
}