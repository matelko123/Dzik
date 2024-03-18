namespace Contracts.Identity.RoleClaims;

public sealed record RoleClaimDto(List<RoleClaimResponse> Claims);

public sealed record RoleClaimResponse(
    string Type,
    string Value,
    string Group,
    string? Description
    );

public sealed record RoleClaimRequest(Guid RoleId, List<string> Permissions);