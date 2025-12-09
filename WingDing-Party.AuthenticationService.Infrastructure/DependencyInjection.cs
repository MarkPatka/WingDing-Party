using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Authentication;
using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Persistence;
using WingDing_Party.AuthenticationService.Application.Common.Interfaces.Services;
using WingDing_Party.AuthenticationService.Infrastructure.Authentication;
using WingDing_Party.AuthenticationService.Infrastructure.Persistence;
using WingDing_Party.AuthenticationService.Infrastructure.Services;

namespace WingDing_Party.AuthenticationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            ConfigurationManager configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
           
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}