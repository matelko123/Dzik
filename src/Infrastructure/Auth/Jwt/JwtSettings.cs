using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Auth.Jwt;

public class JwtSettings : IValidatableObject
{
    public const string SectionName = "JwtSettings";
    
    public string SigningKey { get; set; } = string.Empty;
    public TimeSpan? TokenExpiration { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(SigningKey))
        {
            yield return new ValidationResult("No Key defined in JwtSettings config", new[] { nameof(SigningKey) });
        }
    }
}