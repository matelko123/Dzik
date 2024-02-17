using Domain.Entities.Identity;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Authorization.Constants.Permission;
using Shared.Authorization.Constants.Role;

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
    
    private async Task SeedRolesAsync(AppDbContext dbContext)
    {
        foreach (string roleName in RoleConstants.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not AppRole role)
            {
                // Create the role
                _logger.LogInformation("Seeding {role} Role", roleName);
                role = new AppRole(roleName, $"{roleName} Role.");
                await _roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == RoleConstants.BasicRole)
            {
                await AssignPermissionsToRoleAsync(dbContext, FSHPermissions.Basic, role);
            }
            else if (roleName == RoleConstants.AdministratorRole)
            {
                await AssignPermissionsToRoleAsync(dbContext, FSHPermissions.Admin, role);
                await AssignPermissionsToRoleAsync(dbContext, FSHPermissions.Root, role);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(AppDbContext dbContext, IReadOnlyList<FSHPermission> permissions, AppRole role)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(c => c.Type == ApplicationClaimTypes.Permission && c.Value == permission.Name))
            {
                _logger.LogInformation("Seeding {role} Permission '{permission}'", role.Name, permission.Name);
                dbContext.RoleClaims.Add(new AppRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = ApplicationClaimTypes.Permission,
                    ClaimValue = permission.Name,
                    CreatedBy = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Group = permission.Resource,
                    Description = permission.Description,
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private Task SeedAdminUserAsync()
    {
        return Task.CompletedTask;
    }
}