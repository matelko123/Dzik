using Application.Errors;
using Application.Identity.Roles;
using Contracts.Identity.Roles;
using Domain.Entities.Identity;
using Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization;
using Shared.Authorization.Constants.Permission;
using Shared.Authorization.Constants.Role;
using Shared.Wrapper;
using System.Data;
using System.Security.Claims;
using System.Xml.Linq;

namespace Infrastructure.Identity;

public class RoleService(
    AppDbContext appDbContext,
    RoleManager<AppRole> roleManager, 
    UserManager<AppUser> userManager)
    : IRoleService
{
    public async Task<Result<List<RoleDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        return Result<List<RoleDto>>.Success(roles.Adapt<List<RoleDto>>());
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        return await roleManager.Roles.CountAsync(cancellationToken);
    }

    public async Task<Result<RoleDto>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());

        return role is null
            ? Result<RoleDto>.Error(RoleErrors.NotFound)
            : Result<RoleDto>.Success(role.Adapt<RoleDto>());
    }

    public async Task<Result<RoleDto>> GetByIdWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var existingRole = await roleManager.FindByIdAsync(roleId.ToString());
        if (existingRole is null)
        {
            return Result<RoleDto>.Error(RoleErrors.NotFound);
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

    public async Task<Result<RoleDto>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(name);

        return role is null
            ? Result<RoleDto>.Error(RoleErrors.NotFound)
            : Result<RoleDto>.Success(role.Adapt<RoleDto>());
    }

    public async Task<Result> CreateAsync(AppRole role, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(role.Name))
        {
            return Result.Error(RoleErrors.Failed);
        }

        if (await roleManager.FindByNameAsync(role.Name) is { })
        {
            return Result.Error(RoleErrors.AlreadyExists);
        }

        IdentityResult result = await roleManager.CreateAsync(role);

        return result.Succeeded
            ? Result.Created()
            : Result<AppRole>.Error(result.GetErrors());
    }

    public async Task<Result> UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (existingRole is null)
        {
            return Result<RoleDto>.Error(RoleErrors.NotFound);
        }

        if (existingRole.Name == RoleConstants.AdministratorRole)
        {
            return Result.Error(RoleErrors.NotAllowed);
        }

        existingRole.Name = request.Name;
        existingRole.NormalizedName = request.Name.ToUpperInvariant();
        existingRole.Description = request.Description;
        var result = await roleManager.UpdateAsync(existingRole);

        if (!result.Succeeded)
        {
            return Result.Error(result.GetErrors());
        }

        var currentClaims = await roleManager.GetClaimsAsync(existingRole);

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !request.Permissions.Contains(c.Value)))
        {
            var removeResult = await roleManager.RemoveClaimAsync(existingRole, claim);
            if (!removeResult.Succeeded)
            {
                return Result.Error(removeResult.GetErrors());
            }
        }

        // Add all permissions that were not previously selected
        var allPermissions = FSHPermissions.All;
        var permissionsToAdd = request.Permissions
            .Where(c => !currentClaims.Select(x => x.Value).Contains(c))
            .Where(c => allPermissions.Select(x => x.Name).Contains(c));

        foreach (string permission in permissionsToAdd)
        {
            var fshPermission = FSHPermissions.All.FirstOrDefault(x => x.Name == permission);
            if(fshPermission is null) continue;

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

    public async Task<Result> DeleteAsync(Guid roleId, CancellationToken cancellationToken)
    {
        AppRole? existingRole = await roleManager.FindByIdAsync(roleId.ToString());
        if (existingRole is null)
        {
            return Result<AppRole>.Error(RoleErrors.NotFound);
        }
        
        if (AppRoles.IsDefault(existingRole.Name!))
        {
            return Result<AppRole>.Error(RoleErrors.NotAllowed);
        }
        
        if ((await userManager.GetUsersInRoleAsync(existingRole.Name!)).Any())
        {
            return Result<AppRole>.Error(RoleErrors.NotAllowedBeingUsed);
        }

        var deleteResult = await roleManager.DeleteAsync(existingRole);
        return deleteResult.Succeeded
            ? Result.NoContent()
            : Result.Error(deleteResult.GetErrors());
    }

    public Result<List<FSHPermission>> GetAllPermissions(CancellationToken cancellationToken = default)
    {
        var allPermissions = FSHPermissions.All.ToList();
        return Result<List<FSHPermission>>.Success(allPermissions);
    }
}