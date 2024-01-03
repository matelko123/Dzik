namespace Contracts.Identity.Authentication;

public record UserDto(
    Guid Id,
    string? FirstName,
    string? LastName,
    string? UserName,
    string? Email);