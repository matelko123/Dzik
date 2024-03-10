using Api.FunctionalTests.Abstractions;
using Api.FunctionalTests.Extensions;
using Application.Errors;
using Contracts.Identity.Roles;
using FluentAssertions;
using Shared.Authorization.Constants.Role;
using System.Net;
using System.Net.Http.Json;

namespace Api.FunctionalTests.Identity.Roles;

public class CreateRoleTests(
    FunctionalTestWebAppFactory factory) 
    : BaseFunctionalTest(factory)
{
    private const string BASE_ROUTE = "api/roles";

    [Fact]
    public async Task Should_ReturnBadRequest_WhenRoleExists()
    {
        // Arrange
        var request = new CreateRoleRequest(RoleConstants.AdministratorRole, "test description");

        // Act
        var response = await HttpClient.PostAsJsonAsync(BASE_ROUTE, request);

        // Assert
        response.StatusCode.Should().Be(RoleErrors.AlreadyExists.StatusCode);
        var result = await response.GetResult()!;
        result.Errors.Should().BeEquivalentTo(RoleErrors.AlreadyExists.Message);
        result.Status.Should().Be(RoleErrors.AlreadyExists.StatusCode);
    }

    [Fact]
    public async Task Should_Create_WhenRoleValid()
    {
        // Arrange
        var request = new CreateRoleRequest("Viewer", "test description");

        // Act
        var response = await HttpClient.PostAsJsonAsync(BASE_ROUTE, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
