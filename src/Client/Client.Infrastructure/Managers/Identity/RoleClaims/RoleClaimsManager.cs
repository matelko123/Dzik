using Contracts.Identity.RoleClaims;
using Shared.Wrapper;

namespace Client.Infrastructure.Managers.Identity.RoleClaims;

public class RoleClaimsManager(IRoleClaimsClient client) : IRoleClaimsManager
{
    public async Task<Result<RoleClaimDto>> GetAllRoleClaims()
    {
        return await client.GetAllRoleClaims();
    }

    public async Task<Result<RoleClaimDto>> GetRoleClaimByRoleId(Guid id)
    {
        return await client.GetRoleClaimByRoleId(id);
    }

    public async Task<Result> UpdateRoleClaims(Guid RoleId, List<string> Permissions)
    {
        return await client.UpdateRoleClaims(RoleId, new RoleClaimRequest(RoleId, Permissions));
    }
}
