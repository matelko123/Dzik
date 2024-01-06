using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Initialization;
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
            .AddDbContext<AppDbContext>(opt =>
            {
                var connectionString = configuration.GetConnectionString("Default")
                                       ?? throw new Exception("Empty connection string 'Default'");
                opt.UseNpgsql(connectionString);
            })
            .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
            .AddTransient<ApplicationDbInitializer>()
            .AddTransient<ApplicationDbSeeder>();
    }
}