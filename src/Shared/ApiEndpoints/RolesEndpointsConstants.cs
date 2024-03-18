namespace Shared.ApiEndpoints;

public static class RolesEndpointsConstants
{
    public const string Base = "api/identity/roles";

    public const string GetAllRoles = "/";
    public const string GetRoleById = $"/{{id}}";
    public const string GetUsersByRoleId = $"/{{id}}/users";
    public const string CreateRole = "/";
    public const string DeleteRole = $"/{{id}}";
    public const string UpdateRole = $"/{{id}}";
}
