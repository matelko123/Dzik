using System.Net;
using Api.FunctionalTests.Abstractions;
using Api.FunctionalTests.Extensions;
using Application.Errors;
using Contracts.Identity.Roles;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization.Constants.Role;

namespace Api.FunctionalTests.Identity.Roles;

public class GetRoleTests(
    FunctionalTestWebAppFactory factory) 
    : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Should_ReturnAllRoles_WhenExists()
    {
        // Arrange

        // Act
        var response = await HttpClient.GetAsync("roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.GetResult<List<RoleDto>>()!;
        result.Errors.Should().BeEmpty();
        result.Status.Should().Be(HttpStatusCode.OK);
        result.Value!.Count.Should().Be(2);
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenRoleNotExists()
    {
        // Arrange
        Guid requestRoleId = Guid.NewGuid();

        // Act
        var response = await HttpClient.GetAsync($"roles/{requestRoleId}");

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
    public async Task Should_ReturnRole_WhenExist(string roleName)
    {
        // Arrange
        var role = await DbContext.Roles.SingleAsync(x => x.Name == roleName)!;

        // Act
        var response = await HttpClient.GetAsync($"roles/{role.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.GetResult<RoleDto>()!;
        result.Errors.Should().BeEmpty();
        result.Status.Should().Be(HttpStatusCode.OK);
        result.Value!.Name.Should().Be(roleName);
    }
}
