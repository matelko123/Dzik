using Contracts.Identity.RoleClaims;
using Refit;
using Shared.Wrapper;

namespace Client.Infrastructure.Managers.Identity.RoleClaims;

public interface IRoleClaimsClient
{
    [Get("/")]
    Task<Result<RoleClaimDto>> GetAllRoleClaims();

    [Get("/{id}")]
    Task<Result<RoleClaimDto>> GetRoleClaimByRoleId(Guid id);
    

    [Put("/{id}")]
    Task<Result> UpdateRoleClaims(Guid id, RoleClaimRequest request);
}
