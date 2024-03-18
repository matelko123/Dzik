using Contracts.Identity.RoleClaims;
using Shared.Wrapper;

namespace Client.Infrastructure.Managers.Identity.RoleClaims;

public interface IRoleClaimsManager
{
    Task<Result<RoleClaimDto>> GetAllRoleClaims();
    Task<Result<RoleClaimDto>> GetRoleClaimByRoleId(Guid id);
    Task<Result> UpdateRoleClaims(Guid RoleId, List<string> Permissions);
}
