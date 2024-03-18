using Contracts.Identity.Authentication;
using Contracts.Identity.Roles;
using Shared.Wrapper;

namespace Client.Infrastructure.Managers.Identity.Roles;

public interface IRoleManager
{
    Task<Result<List<RoleDto>>> GetAllRoles();
    Task<Result<RoleDto>> GetRoleById(Guid id); 
    Task<Result<List<UserDto>>> GetUsersByRoleAsync(Guid id);
    Task<Result> CreateRole(RoleDto request);
    Task<Result> DeleteRole(Guid id);
    Task<Result> UpdateRole(RoleDto request);
}
