namespace Contracts.Identity.Roles;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<RoleClaimResponse>? Permissions { get; set; }
}


public sealed record RoleClaimResponse (
    string Type,
    string Value,
    string Group,
    string? Description
    );
