using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public sealed class AppRole : IdentityRole<Guid>
{
    public string? Description { get; set; }

    public AppRole(string name, string? description = null)
        : base(name)
    {
        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }
}