using Api.FunctionalTests.Abstractions;

namespace Api.FunctionalTests.Identity.Roles;

public partial class RoleTests(
    FunctionalTestWebAppFactory factory) 
    : BaseFunctionalTest(factory)
{
    private const string BASE_ROUTE = "api/identity/roles";
}
