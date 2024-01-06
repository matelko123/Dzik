using Application.Errors;
using Application.Exceptions;
using Application.Features.Identity.Authentication.Commands;
using FluentAssertions.Specialized;

namespace Application.IntegrationTests.Identity.AuthEndpointsTests;

public class CreateUserTests : BaseIntegrationTest
{
    public CreateUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Fact]
    public async Task CreateUser_ShouldAddUser_WhenCommandIsValid()
    {
        // Arrange
        RegisterCommand command = RegisterCommandBuilder.Create();
        
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
        RegisterCommand command = RegisterCommandBuilder.Create() with { Email = "test" };
        ValidationError[] expectedErrors = [new ValidationError(
            "Email", 
            UserErrors.Validation.Email.InvalidFormat)];

        // Act
        Func<Task> act = () => Sender.Send(command, default);
        
        // Assert
        ExceptionAssertions<ValidationException>? e = await act.Should().ThrowAsync<ValidationException>();
        e.WithMessage("One or more validation errors occurs");
        e.Subject.Single().Errors.Should().BeEquivalentTo(expectedErrors);
    }
    
    [Fact]
    public async Task CreateUser_ShouldThrowError_WhenPasswordIsTooShort()
    {
        // Arrange
        RegisterCommand command = RegisterCommandBuilder.Create() with { Password = "123" };
        ValidationError[] expectedErrors = [new ValidationError(
            "Password", 
            UserErrors.Validation.Password.MinimumLengthMessage(6))]; //TODO: Change length with IdentityOptions PasswordOptions
    
        // Act
        Func<Task> act = () => Sender.Send(command, default);
    
        // Assert
        ExceptionAssertions<ValidationException>? e = await act.Should().ThrowAsync<ValidationException>();
        e.WithMessage("One or more validation errors occurs");
        e.Subject.Single().Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowError_WhenUsernameIsEmpty()
    {
        // Arrange
        RegisterCommand command = RegisterCommandBuilder.Create() with { UserName = "" };
        ValidationError[] expectedErrors = [new ValidationError(
            "UserName", 
            UserErrors.Validation.Username.NotEmpty)];
    
        // Act
        Func<Task> act = () => Sender.Send(command, default);
    
        // Assert
        ExceptionAssertions<ValidationException>? e = await act.Should().ThrowAsync<ValidationException>();
        e.WithMessage("One or more validation errors occurs");
        e.Subject.Single().Errors.Should().BeEquivalentTo(expectedErrors);
    }
}