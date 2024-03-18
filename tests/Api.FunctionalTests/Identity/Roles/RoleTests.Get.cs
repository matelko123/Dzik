using Application.Errors;
using Contracts.Identity.Roles;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization.Constants.Role;
using System.Net;

namespace Api.FunctionalTests.Identity.Roles;

public partial class RoleTests
{
    [Fact]
    public async Task Get_Should_ReturnAllRoles_WhenExists()
    {
        // Arrange

        // Act
        var response = await HttpClient.GetAsync(BASE_ROUTE);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.GetResult<List<RoleDto>>()!;
        result.Errors.Should().BeEmpty();
        result.Status.Should().Be(HttpStatusCode.OK);
        result.Value!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Get_Should_ReturnNotFound_WhenRoleNotExists()
    {
        // Arrange
        Guid requestRoleId = Guid.NewGuid();

        // Act
        var response = await HttpClient.GetAsync($"{BASE_ROUTE}/{requestRoleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.GetResult<RoleDto>();
        result.Errors.Should().BeEquivalentTo(RoleErrors.NotFound.Message);
        result.Status.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
    }

    [Theory]
    [InlineData(RoleConstants.BasicRole)]
    [InlineData(RoleConstants.AdministratorRole)]
    public async Task Get_Should_ReturnRole_WhenExist(string roleName)
    {
        // Arrange
        var role = await DbContext.Roles.SingleAsync(x => x.Name == roleName)!;

        // Act
        var response = await HttpClient.GetAsync($"{BASE_ROUTE}/{role.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.GetResult<RoleDto>()!;
        result.Errors.Should().BeEmpty();
        result.Status.Should().Be(HttpStatusCode.OK);
        result.Value!.Name.Should().Be(roleName);
    }
}
