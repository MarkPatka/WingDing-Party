using AuthService.Application.UserManagement.Command.AssignRole;
using AuthService.Contracts.Authentication;
using Mapster;

namespace AuthService.Api.Common.Mapping;

/// <summary>
/// Maps the route-derived <see cref="AssignRoleRequest.Id"/> onto the command's
/// UserId (names differ, so Mapster's convention mapping would leave it empty).
/// </summary>
public sealed class AssignRoleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
        => config.NewConfig<AssignRoleRequest, AssignRoleCommand>()
            .Map(dest => dest.UserId, src => src.Id);
}
