using Api.FunctionalTests.Abstractions;
using Application.Errors;
using Contracts.Common;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Api.FunctionalTests.Identity.Auth;

public class RegisterUserTests(
    FunctionalTestWebAppFactory factory)
    : BaseFunctionalTest(factory)
{
    private const string BASE_ROUTE = "api/auth/register";

    [Fact]
    public async Task Should_ReturnBadRequest_WhenUserInvalid()
    {
        // Arrange
        RegisterRequest request = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        var expectingErrors = new Dictionary<string, string[]>
        {
            { "FirstName", ["'First Name' must not be empty."] },
            { "LastName", ["'Last Name' must not be empty."] },
            { "Email", ["'Email' must not be empty."] },
            { "UserName", ["'User Name' must not be empty."] },
            { "Password", ["'Password' must not be empty."] },
            { "PhoneNumber", ["'Phone Number' must not be empty."] }
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(BASE_ROUTE, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResult result = await response.GetResponse<ErrorResult>();
        result.Should().BeEquivalentTo(ResultsExtensions.ValidationErrorResult(expectingErrors));
    }

    [Fact]
    public async Task Should_ReturnBadRequest_WhenUserEmailTaken()
    {
        // Arrange
        string email = $"{RandomString(6)}@test.pl";
        AppUser user = new() { Email = email, NormalizedEmail = email.ToUpper() };
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        RegisterRequest request = GenerateRequest(email: email);
        var expectingErrors = new Dictionary<string, string[]>
        {
            { "Email", [UserErrors.Validation.EmailAlreadyTaken] }
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(BASE_ROUTE, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResult result = await response.GetResponse<ErrorResult>();
        result.Should().BeEquivalentTo(ResultsExtensions.ValidationErrorResult(expectingErrors));
    }


    [Fact]
    public async Task Should_ReturnBadRequest_WhenUserNameTaken()
    {
        // Arrange
        string userName = RandomString(6);
        AppUser user = new() { UserName = userName, NormalizedUserName = userName.ToUpper() };
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        RegisterRequest request = GenerateRequest(userName: userName);
        var expectingErrors = new Dictionary<string, string[]>
        {
            { "UserName", [UserErrors.Validation.UsernameAlreadyTaken] }
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(BASE_ROUTE, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResult result = await response.GetResponse<ErrorResult>();
        result.Should().BeEquivalentTo(ResultsExtensions.ValidationErrorResult(expectingErrors));
    }


    [Fact]
    public async Task Should_ReturnBadRequest_WhenPhoneNumberTaken()
    {
        // Arrange
        const string phoneNumber = "123123123";
        AppUser user = new() { UserName = RandomString(6), PhoneNumber = phoneNumber };
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        RegisterRequest request = GenerateRequest(phoneNumber: phoneNumber);
        var expectingErrors = new Dictionary<string, string[]>
        {
            { "PhoneNumber", [UserErrors.Validation.PhoneNumberAlreadyTaken] }
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(BASE_ROUTE, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResult result = await response.GetResponse<ErrorResult>();
        result.Should().BeEquivalentTo(ResultsExtensions.ValidationErrorResult(expectingErrors));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenValid()
    {
        // Arrange
        RegisterRequest request = GenerateRequest();

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(BASE_ROUTE, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.GetResult<Guid>();
        result.Errors.Should().BeEmpty();
        result.Value.Should().NotBeEmpty();
        result.ValueType.Should().Be(typeof(Guid));
    }


    private static RegisterRequest GenerateRequest(
        string? firstName = null,
        string? lastName = null,
        string? userName = null,
        string? email = null,
        string? password = null,
        string? phoneNumber = null)
    {
        return new RegisterRequest(
            firstName ?? RandomString(6),
            lastName ?? RandomString(6),
            userName ?? RandomString(6),
            email ?? $"{RandomString(6)}@test.pl",
            password ?? RandomString(6),
            phoneNumber ?? RandomNumeric());
    }
}