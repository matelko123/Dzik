using Host.Endpoints.Internal;

namespace Host.Endpoints.Identity;


public class UserEndpoints : IEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Auth";
    private const string BaseRoute = "auth";

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
    }
}
