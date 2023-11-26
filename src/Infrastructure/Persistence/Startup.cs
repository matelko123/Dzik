using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

internal static class Startup
{
    internal static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddDbContext<BaseDbContext>(opt =>
            {
                var connectionString = configuration.GetConnectionString("Default")
                                       ?? throw new Exception("Empty connection string 'Default'");
                opt.UseSqlite(connectionString);
            });
    }
}