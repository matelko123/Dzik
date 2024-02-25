using Api.FunctionalTests.Abstractions;
using Api.FunctionalTests.Extensions;
using Application.Errors;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization.Constants.Role;
using System.Net;

namespace Api.FunctionalTests.Identity.Roles;

public class DeleteRoleTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Theory]
    [InlineData(RoleConstants.BasicRole)]
    [InlineData(RoleConstants.AdministratorRole)]
    public async Task Should_ReturnBadRequest_WhenRoleDefault(string roleName)
    {
        // Arrange
        var adminRole = await DbContext.Roles.SingleOrDefaultAsync(x => x.Name == roleName);

        // Act
        var response = await HttpClient.DeleteAsync($"roles/{adminRole!.Id}");
        var isStillInDb = await DbContext.Roles.AnyAsync(x => x.Name == roleName);

        // Assert
        response.StatusCode.Should().Be(RoleErrors.NotAllowed.StatusCode);
        var result = await response.GetResult();
        result.Status.Should().Be(RoleErrors.NotAllowed.StatusCode);
        result.Errors.Should().BeEquivalentTo(RoleErrors.NotAllowed.Message);

        isStillInDb.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenRoleNotExists()
    {
        // Arrange
        Guid roleId = Guid.NewGuid();

        // Act
        var response = await HttpClient.DeleteAsync($"roles/{roleId}");

        // Assert
        response.StatusCode.Should().Be(RoleErrors.NotFound.StatusCode);
        var result = await response.GetResult();
        result.Status.Should().Be(RoleErrors.NotFound.StatusCode);
        result.Errors.Should().BeEquivalentTo(RoleErrors.NotFound.Message);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_WhenRoleIsBeingUsed()
    {
        // Arrange
        var role = new AppRole("Test", "Testing role");
        var user = new AppUser();
        await DbContext.Roles.AddAsync(role);
        await DbContext.Users.AddAsync(user);
        var userRole = new Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>()
            { UserId = user.Id, RoleId = role.Id };
        await DbContext.UserRoles.AddAsync(userRole);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.DeleteAsync($"roles/{role.Id}");

        // Assert
        response.StatusCode.Should().Be(RoleErrors.NotAllowedBeingUsed.StatusCode);
        var result = await response.GetResult();
        result.Status.Should().Be(RoleErrors.NotAllowedBeingUsed.StatusCode);
        result.Errors.Should().BeEquivalentTo(RoleErrors.NotAllowedBeingUsed.Message);
    }


    [Fact]
    public async Task Should_ReturnSuccess_WhenRoleValid()
    {
        // Arrange
        var role = new AppRole("Test", "Testing role");
        await DbContext.Roles.AddAsync(role);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.DeleteAsync($"roles/{role.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
