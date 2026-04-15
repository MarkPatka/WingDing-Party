using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.Application.Common.Mapping;

public static class MappingRegistration
{
    public static IServiceCollection AddMappings(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        var config = new TypeAdapterConfig();
        foreach (var assembly in assembliesToScan)
        {
            config.Scan(assembly);
        }
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}
