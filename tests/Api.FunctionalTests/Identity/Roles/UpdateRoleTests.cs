using System.Net;
using System.Net.Http.Json;
using Api.FunctionalTests.Abstractions;
using Api.FunctionalTests.Extensions;
using Application.Errors;
using Contracts.Identity.Roles;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization.Constants.Permission;
using Shared.Authorization.Constants.Role;

namespace Api.FunctionalTests.Identity.Roles;

public class UpdateRoleTests(
    FunctionalTestWebAppFactory factory)
    : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Should_ReturnNotFound_WhenRoleNotExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var updatedRole = new AppRole("Test 1", "Test role 1");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/roles/{roleId}", updatedRole);

        // Assert
        response.StatusCode.Should().Be(RoleErrors.NotFound.StatusCode);
        var result = await response.GetResult();
        result.Status.Should().Be(RoleErrors.NotFound.StatusCode);
        result.Errors.Should().BeEquivalentTo(RoleErrors.NotFound.Message);
    }

    [Fact]
    public async Task Should_ReturnNotAllowed_WhenRoleIsAdmin()
    {
        // Arrange
        var roleAdmin = await DbContext.Roles.SingleAsync(x => x.Name == RoleConstants.AdministratorRole);

        var updatedRole = new AppRole("Test 1", "Test role 1");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/roles/{roleAdmin.Id}", updatedRole);

        // Assert
        response.StatusCode.Should().Be(RoleErrors.NotAllowed.StatusCode);
        var result = await response.GetResult();
        result.Status.Should().Be(RoleErrors.NotAllowed.StatusCode);
        result.Errors.Should().BeEquivalentTo(RoleErrors.NotAllowed.Message);
    }

    [Fact]
    public async Task Should_UpdateRoleWithoutPermissions_WhenValid()
    {
        // Arrange
        var role = new AppRole(Guid.NewGuid().ToString(), "Test role");
        await DbContext.Roles.AddAsync(role);
        await DbContext.SaveChangesAsync();

        var updatedRole = new UpdateRoleRequest(role.Id, Guid.NewGuid().ToString(), "Test role 1", []);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/roles/{role.Id}", updatedRole);
        var roleClaims = await DbContext.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        roleClaims.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_UpdateRoleWithPermissions_WhenValid()
    {
        // Arrange
        var role = new AppRole(Guid.NewGuid().ToString(), "Test role");
        await DbContext.Roles.AddAsync(role);
        await DbContext.SaveChangesAsync();

        var rolePermissions = FSHPermissions.Admin.Select(x => x.Name).ToList();
        var updatedRole = new UpdateRoleRequest(role.Id, Guid.NewGuid().ToString(), "Test role 1", rolePermissions);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/roles/{role.Id}", updatedRole);
        var roleClaims = await DbContext.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        roleClaims
            .Where(x => x.ClaimType == ApplicationClaimTypes.Permission)
            .Select(x => x.ClaimValue)
            .Should().BeEquivalentTo(rolePermissions);
    }
}

