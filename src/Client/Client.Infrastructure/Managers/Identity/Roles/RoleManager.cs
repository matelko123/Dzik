using Contracts.Identity.Roles;
using Shared.Wrapper;

namespace Client.Infrastructure.Managers.Identity.Roles;

public class RoleManager(IRolesClient client) : IRoleManager
{
    public async Task<Result> CreateRole(CreateRoleRequest request)
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
    public async Task<Result> UpdateRole(Guid id, UpdateRoleRequest request)
    {
        return await client.UpdateRole(id, request);
    }
}
