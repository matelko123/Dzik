using Application.Errors;
using Application.Identity.Roles;
using Contracts.Identity.Roles;
using Domain.Entities.Identity;
using Infrastructure.Persistence.Context;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization;
using Shared.Authorization.Constants.Permission;
using Shared.Authorization.Constants.Role;
using Shared.Wrapper;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Infrastructure.Identity;

public class RoleService(
    AppDbContext appDbContext,
    RoleManager<AppRole> roleManager, 
    UserManager<AppUser> userManager)
    : IRoleService
{
    public async Task<Result<List<AppRole>>> GetListAsync(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        return Result<List<AppRole>>.Success(roles);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        return await roleManager.Roles.CountAsync(cancellationToken);
    }

    public async Task<Result<AppRole>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());

        return role is null
            ? Result<AppRole>.Fail(RoleErrors.NotFound)
            : Result<AppRole>.Success(role);
    }

    public async Task<Result<RoleDto>> GetByIdWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var existingRole = await roleManager.FindByIdAsync(roleId.ToString());
        if (existingRole is null)
        {
            return Result<RoleDto>.Fail(RoleErrors.NotFound);
        }

        IList<Claim> roleClaims = await roleManager.GetClaimsAsync(existingRole);

        var response = existingRole.Adapt<RoleDto>();
        response.Permissions = new List<RoleClaimResponse>(await appDbContext
            .RoleClaims
            .Where(r => r.RoleId == existingRole.Id)
            .Select(rc => new RoleClaimResponse
            (
                rc.ClaimType!,
                rc.ClaimValue!,
                rc.Group!,
                rc.Description
            ))
            .ToListAsync(cancellationToken));

        return Result<RoleDto>.Success(response);
    }

    public async Task<Result<AppRole>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(name);

        return role is null
            ? Result<AppRole>.Fail(RoleErrors.NotFound)
            : Result<AppRole>.Success(role);
    }

    public async Task<Result> CreateAsync(AppRole role, CancellationToken cancellationToken)
    {
        IdentityResult result = await roleManager.CreateAsync(role);

        return result.Succeeded
            ? Result.Success()
            : Result<AppRole>.Fail(result.GetErrors());
    }

    public async Task<Result> UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (existingRole is null)
        {
            return Result<RoleDto>.Fail(RoleErrors.NotFound);
        }

        if (existingRole.Name == RoleConstants.AdministratorRole)
        {
            return Result.Fail(RoleErrors.NotAllowed);
        }

        existingRole.Name = request.Name;
        existingRole.NormalizedName = request.Name.ToUpperInvariant();
        existingRole.Description = request.Description;
        var result = await roleManager.UpdateAsync(existingRole);

        if (!result.Succeeded)
        {
            return Result.Fail(result.GetErrors());
        }

        var currentClaims = await roleManager.GetClaimsAsync(existingRole);

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !request.Permissions.Any(p => p == c.Value)))
        {
            var removeResult = await roleManager.RemoveClaimAsync(existingRole, claim);
            if (!removeResult.Succeeded)
            {
                return Result.Fail(removeResult.GetErrors());
            }
        }

        // Add all permissions that were not previously selected
        var allPermissions = FSHPermissions.All;
        var permissionsToAdd = request.Permissions
            .Where(c => !currentClaims.Any(p => p.Value == c))
            .Where(c => allPermissions.Select(x => x.Name).Contains(c));

        foreach (string permission in permissionsToAdd)
        {
            if (!string.IsNullOrEmpty(permission))
            {
                appDbContext.RoleClaims.Add(new AppRoleClaim
                {
                    RoleId = existingRole.Id,
                    ClaimType = ApplicationClaimTypes.Permission,
                    ClaimValue = permission,
                });
                await appDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        return await Result.SuccessAsync();
    }

    public async Task<Result> DeleteAsync(Guid roleId, CancellationToken cancellationToken)
    {
        AppRole? existingRole = await roleManager.FindByIdAsync(roleId.ToString());
        if (existingRole is null)
        {
            return await Result<AppRole>.FailAsync(RoleErrors.NotFound);
        }
        
        if (AppRoles.IsDefault(existingRole.Name!))
        {
            return await Result<AppRole>.FailAsync(RoleErrors.NotAllowed);
        }
        
        if ((await userManager.GetUsersInRoleAsync(existingRole.Name!)).Any())
        {
            return await Result<AppRole>.FailAsync(RoleErrors.NotAllowedBeingUsed);
        }

        var deleteResult = await roleManager.DeleteAsync(existingRole);
        return deleteResult.Succeeded
            ? Result.Success()
            : Result.Fail(deleteResult.GetErrors());
    }

    public async Task<Result<List<FSHPermission>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var allPermissions = FSHPermissions.All.ToList();
        return await Result<List<FSHPermission>>.SuccessAsync(allPermissions);
    }
}