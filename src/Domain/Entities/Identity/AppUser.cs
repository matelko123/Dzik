using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public sealed class AppUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}