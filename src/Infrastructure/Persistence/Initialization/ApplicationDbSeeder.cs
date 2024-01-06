using Domain.Entities.Identity;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Initialization;

internal sealed class ApplicationDbSeeder
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(
        UserManager<AppUser> userManager, 
        RoleManager<AppRole> roleManager, 
        ILogger<ApplicationDbSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }
    
    public async Task SeedDatabaseAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        await SeedRolesAsync(dbContext);
        await SeedAdminUserAsync();
    }
    
    private Task SeedRolesAsync(AppDbContext dbContext)
    {
        return Task.CompletedTask;
    }

    private Task SeedAdminUserAsync()
    {
        return Task.CompletedTask;
    }
}