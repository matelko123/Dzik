using Application.Common.Interfaces;
using Infrastructure.Time;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common;

internal static class Startup
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IClock, Clock>();
    }
}