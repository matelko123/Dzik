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

    public async Task<Result> UpdateAsync(Guid roleId, RoleDto request, CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.FindByIdAsync(roleId.ToString());
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

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid roleId, CancellationToken cancellationToken)
    {
        AppRole? existingRole = await roleManager.FindByIdAsync(roleId.ToString());
        if (existingRole is null)
        {
            return Result<AppRole>.Error(RoleErrors.NotFound);
        }
        
        if (RoleConstants.IsDefault(existingRole.Name!))
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
}