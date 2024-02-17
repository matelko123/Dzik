using System.Collections.ObjectModel;

namespace Shared.Authorization.Constants.Role;

public static class RoleConstants
{
    public const string AdministratorRole = "Administrator";
    public const string BasicRole = "Basic";

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        AdministratorRole,
        BasicRole
    });
}
