using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public sealed class AppRoleClaim : IdentityRoleClaim<Guid>
{
    public Guid? CreatedBy { get; init; }
    public DateTime CreatedOn { get; init; }
    public string Group { get; set; } = default!;
    public string? Description { get; set; }
}