using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Initialization;

internal sealed class ApplicationDbInitializer
{
    private readonly AppDbContext _dbContext;
    private readonly ApplicationDbSeeder _dbSeeder;
    private readonly ILogger<ApplicationDbInitializer> _logger;

    public ApplicationDbInitializer(
        AppDbContext dbContext,
        ApplicationDbSeeder dbSeeder,
        ILogger<ApplicationDbInitializer> logger)
    {
        _dbContext = dbContext;
        _dbSeeder = dbSeeder;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (!await _dbContext.Database.CanConnectAsync(cancellationToken))
        {
            throw new ApplicationException($"Can't connect to database {_dbContext.Connection.Database} with provider {_dbContext.Database.ProviderName}.");
        }
        _logger.LogInformation("Connection to database succeeded.");

        if (!_dbContext.Database.GetMigrations().Any())
        {
            return;
        }

        if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            _logger.LogInformation("Applying migrations");
            await _dbContext.Database.MigrateAsync(cancellationToken);
        }

        await _dbSeeder.SeedDatabaseAsync(_dbContext, cancellationToken);
    }
}