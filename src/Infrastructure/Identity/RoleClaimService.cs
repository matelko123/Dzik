using Application.Errors;
using Application.Identity.RoleClaim;
using Contracts.Identity.RoleClaims;
using Domain.Entities.Identity;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization.Constants.Permission;
using Shared.Wrapper;

namespace Infrastructure.Identity;

public class RoleClaimService(AppDbContext appDbContext) : IRoleClaimService
{
    public async Task<Result<RoleClaimDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roleClaims = await appDbContext.RoleClaims
            .Where(x => x.ClaimType == ApplicationClaimTypes.Permission)
            .Select(x => new RoleClaimResponse(x.ClaimType, x.ClaimValue, x.Group, x.Description))
            .ToListAsync(cancellationToken);

        return Result<RoleClaimDto>.Success(new RoleClaimDto(roleClaims));
    }

    public async Task<Result<RoleClaimDto>> GetAllByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var roleClaims = await appDbContext.RoleClaims
            .Where(x => x.RoleId == roleId)
            .Where(x => x.ClaimType == ApplicationClaimTypes.Permission)
            .Select(x => new RoleClaimResponse(x.ClaimType, x.ClaimValue, x.Group, x.Description))
            .ToListAsync(cancellationToken);

        return Result<RoleClaimDto>.Success(new(roleClaims));
    }

    public async Task<Result> UpdateAsync(RoleClaimRequest request, CancellationToken cancellationToken = default)
    {
        var existingRole = await appDbContext.Roles.FirstOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken);
        if (existingRole is null)
        {
            return Result.Error(RoleErrors.NotFound);
        }
        
        var currentClaims = await appDbContext.RoleClaims
            .Where(x => x.RoleId == request.RoleId)
            .Where(x => x.ClaimType == ApplicationClaimTypes.Permission)
            .ToListAsync(cancellationToken);

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !request.Permissions.Contains(c.ClaimValue!)))
        {
            appDbContext.RoleClaims.Remove(claim);
            await appDbContext.SaveChangesAsync(cancellationToken);
        }

        // Add all permissions that were not previously selected
        var allPermissions = FSHPermissions.All;
        var permissionsToAdd = request.Permissions
            .Where(c => !currentClaims.Select(x => x.ClaimValue).Contains(c))
            .Where(c => allPermissions.Select(x => x.Name).Contains(c));

        foreach (string permission in permissionsToAdd)
        {
            var fshPermission = FSHPermissions.All.FirstOrDefault(x => x.Name == permission);
            if (fshPermission is null) continue;

            appDbContext.RoleClaims.Add(new AppRoleClaim
            {
                RoleId = existingRole.Id,
                ClaimType = ApplicationClaimTypes.Permission,
                ClaimValue = permission,
                Group = fshPermission.Resource
            });
            await appDbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
