using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity;

public sealed class AppRefreshToken
{
    [Key]
    public string Token { get; set; }
    public Guid UserId { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsRevoked { get; set; } = false;

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } 
}