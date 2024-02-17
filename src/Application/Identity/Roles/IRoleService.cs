using Application.Common.Interfaces;
using Contracts.Identity.Roles;
using Domain.Entities.Identity;
using Shared.Authorization.Constants.Permission;
using Shared.Wrapper;

namespace Application.Identity.Roles;

public interface IRoleService : ITransientService
{
    Task<Result<List<AppRole>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<Result<List<FSHPermission>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<AppRole>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<RoleDto>> GetByIdWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<AppRole>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<Result> CreateAsync(AppRole role, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(UpdateRoleRequest role, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);
}