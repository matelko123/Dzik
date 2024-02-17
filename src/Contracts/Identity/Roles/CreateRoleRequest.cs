namespace Contracts.Identity.Roles;

public sealed record CreateRoleRequest(
    string Name,
    string? Description);
