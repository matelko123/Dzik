using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Persistence;

public class DatabaseSettings : IValidatableObject
{
    public string DbProvider { get; init; } = string.Empty;
    public string ConnectionString { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(DbProvider))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(DbProvider)} is not configured",
                new[] { nameof(DbProvider) });
        }

        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(ConnectionString)} is not configured",
                new[] { nameof(ConnectionString) });
        }
    }
}