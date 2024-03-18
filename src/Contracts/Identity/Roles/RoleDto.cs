using System.ComponentModel.DataAnnotations;

namespace Contracts.Identity.Roles;

public class RoleDto
{ 
    public Guid Id { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "Name length can't be less than 8.")]
    public string Name { get; set; } = default!;

    [Required]
    [MinLength(8, ErrorMessage = "Description length can't be less than 8.")]
    public string? Description { get; set; }
}