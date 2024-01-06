using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Initialization;

internal sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeDatabasesAsync(CancellationToken cancellationToken)
    {
        // First create a new scope
        using var scope = _serviceProvider.CreateScope();
        
        // Then run the initialization in the new scope
        await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
            .InitializeAsync(cancellationToken);
    }
}