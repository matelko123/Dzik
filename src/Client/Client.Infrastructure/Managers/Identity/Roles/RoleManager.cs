using Contracts.Identity.Authentication;
using Contracts.Identity.Roles;
using Shared.Wrapper;

namespace Client.Infrastructure.Managers.Identity.Roles;

public class RoleManager(IRolesClient client) : IRoleManager
{
    public async Task<Result> CreateRole(RoleDto request)
    {
        return await client.CreateRole(request);
    }

    public async Task<Result> DeleteRole(Guid id)
    {
        return await client.DeleteRole(id);
    }

    public async Task<Result<List<RoleDto>>> GetAllRoles()
    {
        return await client.GetAllRoles();
    }

    public async Task<Result<RoleDto>> GetRoleById(Guid id)
    {
        return await client.GetRoleById(id);
    }

    public async Task<Result<List<UserDto>>> GetUsersByRoleAsync(Guid id)
    {
        return await client.GetUsersByRoleAsync(id);
    }

    public async Task<Result> UpdateRole(RoleDto request)
    {
        return await client.UpdateRole(request.Id, request);
    }
}
