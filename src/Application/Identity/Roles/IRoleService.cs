using Contracts.Identity.Roles;
using Domain.Entities.Identity;
using Shared.Wrapper;

namespace Application.Identity.Roles;

public interface IRoleService
{
    Task<Result<List<RoleDto>>> GetListAsync(CancellationToken cancellationToken = default);
    
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<RoleDto>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<RoleDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<Result> CreateAsync(AppRole role, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(Guid roleId, RoleDto role, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);
}