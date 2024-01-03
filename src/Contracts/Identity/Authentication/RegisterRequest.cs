namespace Contracts.Identity.Authentication;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string PhoneNumber);