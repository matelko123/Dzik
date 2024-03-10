using Contracts.Identity.Roles;
using Refit;
using Shared.Wrapper;

namespace Client.Infrastructure;

public interface IRolesClient
{
    [Get("/")]
    Task<Result<List<RoleDto>>> GetAllRoles();


    [Get("/{id}")]
    Task<Result<RoleDto>> GetRoleById(Guid id);


    [Post("/")]
    Task<Result> CreateRole(CreateRoleRequest request);


    [Delete("/{id}")]
    Task<Result> DeleteRole(Guid id);


    [Put("/{id}")]
    Task<Result> UpdateRole(Guid id, UpdateRoleRequest request);
}
