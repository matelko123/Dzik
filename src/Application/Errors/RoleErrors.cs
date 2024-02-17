namespace Application.Errors;

public static class RoleErrors
{
    public const string Failed = "Register role failed";
    public const string NotFound = "Role not found.";
    public const string NotAllowed = "Not allowed to modify Role.";
    public const string NotAllowedBeingUsed = "Not allowed to delete Role as it is being used.";
}