using Mapster;
using UserService.Api.Models.Request;
using UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;
using UserService.Application.UserProfileManagement.Command.UpdateUserProfileCommand;
using UserService.Application.UserProfileManagement.Command.UpdateUserProfileInterestsCommand;
using UserService.Application.UserProfileManagement.Common;
using UserService.Application.UserProfileManagement.Queries.GetUserProfileInterestsQuery;
using UserService.Application.UserProfileManagement.Queries.GetUserProfileQuery;
using UserService.Contracts.UserProfiles;

namespace UserService.Api.Mapping;

public class UserProfileMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<GetUserProfileRequest, GetUserProfileQuery>();
        config.NewConfig<GetUserProfileResult, GetUserProfileResponse>();

        config.NewConfig<CreateUserProfileForm, CreateUserProfileRequest>()
            .MapWith(src => new CreateUserProfileRequest(
                src.DisplayName,
                src.Bio,
                src.AvatarUri,
                src.Interests,
                src.BirthDate));
        
        config.NewConfig<CreateUserProfileRequest, CreateUserProfileCommand>()
            .MapWith(src => new CreateUserProfileCommand(
                src.DisplayName,
                src.Bio,
                src.Avatar,
                src.Interests,
                src.BirthDate));
        
        config.NewConfig<CreateUserProfileResult, CreateUserProfileResponse>()
            .MapWith(src => new CreateUserProfileResponse(
                src.Id,
                src.DisplayName,
                src.Bio,
                src.AvatarUri,
                src.Interests,
                src.BirthDate));
        
        //config.NewConfig<CreateUserProfileResult, CreateUserProfileResponse>();

        config.NewConfig<UpdateUserProfileRequest, UpdateUserProfileCommand>();
        config.NewConfig<UpdateUserProfileResult, UpdateUserProfileResponse>();

        config.NewConfig<GetUserProfileInterestsRequest, GetUserProfileInterestsQuery>();
        config.NewConfig<GetUserProfileInterestsResult, GetUserProfileInterestsResponse>();

        config.NewConfig<UpdateUserProfileInterestsRequest, UpdateUserProfileInterestsCommand>();
        config.NewConfig<UpdateUserProfileInterestsResult, UpdateUserProfileInterestsResponse>();
    }
}