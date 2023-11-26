using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

internal static class IdentityResultExtensions
{
    public static string GetErrors(this IdentityResult result) =>
        string.Join(Environment.NewLine, result.Errors.Select(e => e.Description.ToString()).ToList());
}