using Mapster;
using WingDing_Party.AuthenticationService.Application.Authentication.Commands.Register;
using WingDing_Party.AuthenticationService.Application.Authentication.Common;
using WingDing_Party.AuthenticationService.Application.Authentication.Queries.Login;
using WingDing_Party.AuthenticationService.Contracts.Authentication;

namespace WingDing_Party.AuthenticationService.Api.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, RegisterCommand>();
        config.NewConfig<LoginRequest, LoginQuery>();

        config.NewConfig<AuthenticationResult, AuthenticationResponse>()
            .Map(dest => dest, src => src.User);
    }
}
