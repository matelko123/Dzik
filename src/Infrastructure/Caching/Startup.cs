using Application.Abstractions.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Caching;

internal static class Startup
{
    internal static IServiceCollection AddCaching(
        this IServiceCollection services)
    {
        return services
            .AddMemoryCache()
            .AddSingleton<ICacheService, CacheService>();
    }
}