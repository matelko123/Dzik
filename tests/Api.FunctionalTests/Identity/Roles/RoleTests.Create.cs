using Application.Errors;
using Contracts.Identity.Roles;
using FluentAssertions;
using Shared.Authorization.Constants.Role;
using System.Net;
using System.Net.Http.Json;

namespace Api.FunctionalTests.Identity.Roles;

public partial class RoleTests
{
    [Fact]
    public async Task Create_Should_ReturnBadRequest_WhenRoleExists()
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
    public async Task Create_Should_Create_WhenRoleValid()
    {
        // Arrange
        var request = new CreateRoleRequest("Viewer", "test description");

        // Act
        var response = await HttpClient.PostAsJsonAsync(BASE_ROUTE, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
