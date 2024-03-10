using Client.Infrastructure.Managers.Identity.Roles;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Client.Infrastructure;

public static class Startup
{
    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder.AddRefit();

        builder.Services
            .AddManagers();

        return builder;
    }

    private static WebAssemblyHostBuilder AddRefit(this WebAssemblyHostBuilder builder)
    {
        var apiUrl = $"http://localhost:8080/api";

        builder.Services.AddRefitClient<IRolesClient>()
           .ConfigureHttpClient(c => { c.BaseAddress = new Uri($"{apiUrl}/roles"); });

        return builder;
    }

    private static IServiceCollection AddManagers(this IServiceCollection services)
    {
        return services.AddSingleton<IRoleManager, RoleManager>();
    }
}
