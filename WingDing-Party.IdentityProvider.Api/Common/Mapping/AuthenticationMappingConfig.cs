using Mapster;
using WingDing_Party.IdentityProvider.Application.Authentication.Commands.Register;
using WingDing_Party.IdentityProvider.Application.Authentication.Common;
using WingDing_Party.IdentityProvider.Application.Authentication.Queries.Login;
using WingDing_Party.IdentityProvider.Contracts.Authentication;

namespace WingDing_Party.IdentityProvider.Api.Common.Mapping;

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
