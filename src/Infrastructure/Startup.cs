using Infrastructure.Auth;
using Infrastructure.Common;
using Infrastructure.Mapping;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration config)
    {
        MapsterSettings.Configure();
        return services
            .AddAuth(config)
            .AddPersistence(config)
            .AddServices();
    }
}