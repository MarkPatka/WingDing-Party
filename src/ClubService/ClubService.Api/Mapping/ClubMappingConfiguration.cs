using ClubService.Application.ClubManagement.Command.CreateClubCommand;
using ClubService.Application.ClubManagement.Command.DeleteClubCommand;
using ClubService.Application.ClubManagement.Command.JoinClubCommand;
using ClubService.Application.ClubManagement.Command.LeaveClubCommand;
using ClubService.Application.ClubManagement.Command.UpdateClubCommand;
using ClubService.Application.ClubManagement.Common;
using ClubService.Application.ClubManagement.Queries.GetClubMembersQuery;
using ClubService.Application.ClubManagement.Queries.GetClubQuery;
using ClubService.Application.ClubManagement.Queries.GetClubsByUserQuery;
using ClubService.Application.ClubManagement.Queries.SearchClubsQuery;
using ClubService.Contracts.Clubs;
using Mapster;

namespace ClubService.Api.Mapping;

public class ClubMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<GetClubRequest, GetClubQuery>();
        config.NewConfig<GetClubResult, GetClubResponse>();

        config.NewConfig<CreateClubRequest, CreateClubCommand>();
        config.NewConfig<CreateClubResult, CreateClubResponse>();

        config.NewConfig<UpdateClubRequest, UpdateClubCommand>();
        config.NewConfig<UpdateClubResult, UpdateClubResponse>();

        config.NewConfig<DeleteClubRequest, DeleteClubCommand>();
        config.NewConfig<DeleteClubResult, DeleteClubResponse>();

        config.NewConfig<GetClubsByUserRequest, GetClubsByUserQuery>();
        config.NewConfig<GetClubsByUserResult, GetClubsByUserResponse>();

        config.NewConfig<GetClubMembersRequest, GetClubMembersQuery>();
        config.NewConfig<GetClubMembersResult, GetClubMembersResponse>();
        
        config.NewConfig<JoinToClubRequest, JoinToClubCommand>();
        config.NewConfig<JoinToClubResult, JoinToClubResponse>();
        
        config.NewConfig<LeaveClubRequest, LeaveClubCommand>();
        config.NewConfig<LeaveClubResult, LeaveClubResponse>();
        
        config.NewConfig<SearchClubsRequest, SearchClubsQuery>();
        config.NewConfig<SearchClubsResult, SearchClubsResponse>();
    }
}