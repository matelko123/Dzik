using Contracts.Common;
using System.Net;

namespace Application.Errors;

public static class RoleErrors
{
    public static Error Failed => new(HttpStatusCode.BadRequest, "Register role failed.");
    public static Error AlreadyExists => new(HttpStatusCode.BadRequest, "Role already exists.");
    public static Error NotAllowed => new(HttpStatusCode.BadRequest, "Not allowed to modify Role.");
    public static Error NotAllowedBeingUsed => new(HttpStatusCode.UnprocessableContent, "Not allowed to delete Role as it is being used.");
    public static Error NotFound => new(HttpStatusCode.NotFound, "Role not found.");
}