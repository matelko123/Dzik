namespace Application.Identity.Tokens;


public sealed record RefreshTokenRequest(string Token, string RefreshToken);