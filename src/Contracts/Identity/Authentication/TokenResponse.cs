namespace Contracts.Identity.Authentication;

public sealed record TokenResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime);