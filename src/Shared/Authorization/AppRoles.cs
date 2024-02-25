using Shared.Authorization.Constants.Role;
using System.Collections.ObjectModel;

namespace Shared.Authorization;

public static class AppRoles
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);
    
    public static IEnumerable<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        RoleConstants.AdministratorRole,
        RoleConstants.BasicRole
    });
        
    public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}