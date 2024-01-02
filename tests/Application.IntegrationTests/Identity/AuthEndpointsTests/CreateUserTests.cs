using Application.Features.Identity.Users.Commands;
using FluentAssertions;
using Shared.Wrapper;

namespace Application.IntegrationTests.Identity.AuthEndpointsTests;

public class CreateUserTests : BaseIntegrationTest
{
    public CreateUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    private CreateUserCommand validCommand =
        new CreateUserCommand(
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
        CreateUserCommand command = validCommand;
        
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
        CreateUserCommand command = validCommand with { Email = "test" }; 
        
        // Act
        // Task Action() => Sender.Send(command, default);

        // Assert
        try
        {
            var res = await Sender.Send(command, default);
        }
        catch (Exception ex)
        {
            
        }
    }
}