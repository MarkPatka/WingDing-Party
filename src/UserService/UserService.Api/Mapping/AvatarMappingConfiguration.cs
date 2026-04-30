using Mapster;
using UserService.Api.Models.Request;
using UserService.Application.AvatarManagement.Commands.CreateAvatarCommand;
using UserService.Application.AvatarManagement.Common;
using UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;
using UserService.Application.UserProfileManagement.Command.UpdateUserProfileCommand;
using UserService.Application.UserProfileManagement.Command.UpdateUserProfileInterestsCommand;
using UserService.Application.UserProfileManagement.Common;
using UserService.Application.UserProfileManagement.Queries.GetUserProfileInterestsQuery;
using UserService.Application.UserProfileManagement.Queries.GetUserProfileQuery;
using UserService.Contracts.Avatars;
using UserService.Contracts.UserProfiles;
using UserService.Domain.UserProfileAggregate.Entities;

namespace UserService.Api.Mapping;

public class AvatarMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAvatarForm, CreateAvatarRequest>()
            .MapWith(src => new CreateAvatarRequest(
                src.AvatarStream,
                src.FileName,
                src.ContentType,
                src.UserId,
                src.IsDefault,
                src.IsActive
            ));

        config.NewConfig<CreateAvatarRequest, CreateAvatarCommand>()
            .MapWith(src => new CreateAvatarCommand(
                src.AvatarStream,
                src.FileName,
                src.ContentType,
                src.UserId,
                src.IsDefault,
                src.IsActive));

        config.NewConfig<CreateAvatarResult, CreateAvatarResponse>()
            .MapWith(src => new CreateAvatarResponse(
                src.UserId,
                src.Avatar,
                src.IsDefault,
                src.IsActive
            ));
    }
}