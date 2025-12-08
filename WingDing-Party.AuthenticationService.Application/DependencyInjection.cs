using Microsoft.Extensions.DependencyInjection;
using WingDing_Party.AuthenticationService.Application.Authentication;

namespace WingDing_Party.AuthenticationService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, Authentication.AuthenticationService>();
            return services;
        }
    }
}