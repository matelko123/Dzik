using Contracts.Identity.Authentication;
using Contracts.Identity.Roles;
using Refit;
using Shared.ApiEndpoints;
using Shared.Wrapper;

namespace Client.Infrastructure.Managers.Identity.Roles;

public interface IRolesClient
{
    [Get(RolesEndpointsConstants.GetAllRoles)]
    Task<Result<List<RoleDto>>> GetAllRoles();


    [Get(RolesEndpointsConstants.GetRoleById)]
    Task<Result<RoleDto>> GetRoleById(Guid id);

    [Get(RolesEndpointsConstants.GetUsersByRoleId)]
    Task<Result<List<UserDto>>> GetUsersByRoleAsync(Guid id);

    [Post(RolesEndpointsConstants.CreateRole)]
    Task<Result> CreateRole(RoleDto request);


    [Delete(RolesEndpointsConstants.DeleteRole)]
    Task<Result> DeleteRole(Guid id);


    [Put(RolesEndpointsConstants.UpdateRole)]
    Task<Result> UpdateRole(Guid id, RoleDto request);
}
