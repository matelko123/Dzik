namespace Contracts.Identity.Authentication;

public sealed record RefreshTokenRequest (
    string Token, 
    string RefreshToken
    );