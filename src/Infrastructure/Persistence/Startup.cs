using Infrastructure.Common;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Infrastructure.Persistence;

internal static class Startup
{
    private static readonly ILogger _logger = Log.ForContext(typeof(Startup));
    
    internal static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .BindConfiguration(nameof(DatabaseSettings))
            .PostConfigure(databaseSettings =>
            {
                _logger.Information("Current DB Provider: {dbProvider}", databaseSettings.DbProvider);
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services
            .AddDbContext<AppDbContext>((p, m) =>
            {
                DatabaseSettings databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.UseDatabase(databaseSettings.DbProvider, databaseSettings.ConnectionString);
                /*var connectionString = configuration.GetConnectionString("Default")
                                       ?? throw new Exception("Empty connection string 'Default'");
                opt.UseNpgsql(connectionString);*/
            })
            .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
            .AddTransient<ApplicationDbInitializer>()
            .AddTransient<ApplicationDbSeeder>();
    }

    private static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString)
    {
        return dbProvider.ToLowerInvariant() switch
        {
            DbProviderKeys.Npgsql => builder.UseNpgsql(connectionString, e =>
                e.MigrationsAssembly("Migrators.PostgreSQL")),
            /*DbProviderKeys.SqlServer => builder.UseSqlServer(connectionString, e =>
                e.MigrationsAssembly("Migrators.MSSQL")),
            DbProviderKeys.MySql => builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), e =>
                e.MigrationsAssembly("Migrators.MySQL")
                    .SchemaBehavior(MySqlSchemaBehavior.Ignore)),
            DbProviderKeys.Oracle => builder.UseOracle(connectionString, e =>
                e.MigrationsAssembly("Migrators.Oracle")),
            DbProviderKeys.SqLite => builder.UseSqlite(connectionString, e =>
                e.MigrationsAssembly("Migrators.SqLite")),*/
            _ => throw new InvalidOperationException($"DB Provider {dbProvider} is not supported."),
        };
    }
}