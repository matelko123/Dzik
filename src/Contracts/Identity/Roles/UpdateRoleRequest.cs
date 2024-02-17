namespace Contracts.Identity.Roles;

public sealed record UpdateRoleRequest(
    Guid RoleId,
    string Name,
    string? Description,
    List<string> Permissions);
