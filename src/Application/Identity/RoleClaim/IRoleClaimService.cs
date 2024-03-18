using Contracts.Identity.RoleClaims;
using Shared.Wrapper;

namespace Application.Identity.RoleClaim;

public interface IRoleClaimService
{
    Task<Result<RoleClaimDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<RoleClaimDto>> GetAllByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(RoleClaimRequest request, CancellationToken cancellationToken = default);
}
