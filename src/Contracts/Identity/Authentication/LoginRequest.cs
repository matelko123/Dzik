namespace Contracts.Identity.Authentication;

public record LoginRequest(
    string Email, 
    string Password, 
    bool IsPersistent
    );