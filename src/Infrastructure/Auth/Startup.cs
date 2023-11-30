using Infrastructure.Auth.Jwt;
using Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Auth;

internal static class Startup
{
    internal static IServiceCollection AddAuth(
        this IServiceCollection services, 
        IConfiguration config)
    {
        return services
            .AddIdentity()
            .AddJwtAuth();
    }
}