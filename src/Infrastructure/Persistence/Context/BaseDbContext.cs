using System.Data;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public abstract class BaseDbContext(DbContextOptions options)
    : IdentityDbContext<AppUser, AppRole, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, AppRoleClaim, IdentityUserToken<Guid>>(options)
{
    // Used by Dapper
    public IDbConnection Connection => Database.GetDbConnection();

    public DbSet<AppRefreshToken> RefreshTokens => Set<AppRefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        #if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        #endif

        // If you want to see the sql queries that efcore executes:

        // Uncomment the next line to see them in the output window of visual studio
        optionsBuilder.LogTo(m => System.Diagnostics.Debug.WriteLine(m), Microsoft.Extensions.Logging.LogLevel.Information);

        // Or uncomment the next line if you want to see them in the console
        // optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
    }
}