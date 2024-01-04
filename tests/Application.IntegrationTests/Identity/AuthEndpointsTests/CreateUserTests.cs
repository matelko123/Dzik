using Application.Errors;
using Application.Exceptions;
using Application.Features.Identity.Authentication.Commands;
using FluentAssertions;
using Shared.Wrapper;

namespace Application.IntegrationTests.Identity.AuthEndpointsTests;

public class CreateUserTests : BaseIntegrationTest
{
    public CreateUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    private RegisterCommand validCommand =
        new RegisterCommand(
            "Mateusz", 
            "Gutowski", 
            "matelko", 
            "mateusz@gmail.com",
            "qwerty", 
            "123123123");

    [Fact]
    public async Task CreateUser_ShouldAddUser_WhenCommandIsValid()
    {
        // Arrange
        RegisterCommand command = validCommand;
        
        // Act
        Result<Guid> result = await Sender.Send(command, default);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task CreateUser_ShouldThrowError_WhenEmailIsInvalid()
    {
        // Arrange
        RegisterCommand command = validCommand with { Email = "test" };

        // Act
        Func<Task> act = () => Sender.Send(command, default);
        
        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("One or more validation errors occurs");
    }
}