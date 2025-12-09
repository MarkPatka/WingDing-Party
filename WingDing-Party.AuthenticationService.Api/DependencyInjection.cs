using Microsoft.AspNetCore.Mvc.Infrastructure;
using WingDing_Party.AuthenticationService.Api.Common.Errors;

namespace WingDing_Party.AuthenticationService.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddGlobalErrorHandling();
        return services;
    }

    private static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
    {
        services.AddSingleton<ProblemDetailsFactory, AuthenticationServiceProblemDetailsFactory>();
        services.AddProblemDetails();
        return services;
    }
}
