using Application.Errors;
using Application.Exceptions;
using Application.Features.Identity.Authentication.Commands;
using FluentAssertions.Specialized;

namespace Application.IntegrationTests.Identity.AuthEndpointsTests;

public class RegisterCommandTests : BaseIntegrationTest
{
    public RegisterCommandTests(IntegrationTestWebAppFactory factory)
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
    
    [Fact]
    public async Task CreateUser_ShouldThrowError_WhenUsernameAlreadyTaken()
    {
        // Arrange
        RegisterCommand command = RegisterCommandBuilder.Create();
        RegisterCommand command2 = RegisterCommandBuilder.Create() with { UserName = command.UserName };
        ValidationError[] expectedErrors = [new ValidationError(
            "UserName", 
            UserErrors.Validation.Username.AlreadyTaken(command.UserName))];
    
        // Act
        await Sender.Send(command, default);
        Func<Task> act = () => Sender.Send(command2, default);
    
        // Assert
        ExceptionAssertions<ValidationException>? e = await act.Should().ThrowAsync<ValidationException>();
        e.WithMessage("One or more validation errors occurs");
        e.Subject.Single().Errors.Should().BeEquivalentTo(expectedErrors);
    }
    
    [Fact]
    public async Task CreateUser_ShouldThrowError_WhenEmailAlreadyTaken()
    {
        // Arrange
        RegisterCommand command = RegisterCommandBuilder.Create();
        RegisterCommand command2 = RegisterCommandBuilder.Create() with { Email = command.Email };
        ValidationError[] expectedErrors = [new ValidationError(
            "Email", 
            UserErrors.Validation.Email.AlreadyTaken(command.Email))];
    
        // Act
        await Sender.Send(command, default);
        Func<Task> act = () => Sender.Send(command2, default);
    
        // Assert
        ExceptionAssertions<ValidationException>? e = await act.Should().ThrowAsync<ValidationException>();
        e.WithMessage("One or more validation errors occurs");
        e.Subject.Single().Errors.Should().BeEquivalentTo(expectedErrors);
    }
    
    [Fact]
    public async Task CreateUser_ShouldThrowError_WhenPhoneNumberAlreadyTaken()
    {
        // Arrange
        RegisterCommand command = RegisterCommandBuilder.Create();
        RegisterCommand command2 = RegisterCommandBuilder.Create() with { PhoneNumber = command.PhoneNumber };
        ValidationError[] expectedErrors = [new ValidationError(
            "PhoneNumber", 
            UserErrors.Validation.PhoneNumber.AlreadyTaken(command.PhoneNumber))];
    
        // Act
        await Sender.Send(command, default);
        Func<Task> act = () => Sender.Send(command2, default);
    
        // Assert
        ExceptionAssertions<ValidationException>? e = await act.Should().ThrowAsync<ValidationException>();
        e.WithMessage("One or more validation errors occurs");
        e.Subject.Single().Errors.Should().BeEquivalentTo(expectedErrors);
    }
}