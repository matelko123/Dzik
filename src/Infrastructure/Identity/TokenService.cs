using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;
using Application.Identity.Tokens;
using Domain.Entities.Identity;
using Infrastructure.Auth.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Wrapper;

namespace Infrastructure.Identity;

public class TokenService: ITokenService
{
    private readonly IClock _clock;
    private readonly JwtSettings _jwtSettings;
    
    private readonly TimeSpan _expiry;

    public TokenService(
        IClock clock,
        IOptions<JwtSettings> jwtSettings)
    {
        _clock = clock;
        _jwtSettings = jwtSettings.Value;
            
        _expiry = _jwtSettings.TokenExpiration ?? TimeSpan.FromHours(1);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
    
    private static IEnumerable<Claim> GetClaims(AppUser user) =>
        new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };
    
    public TokenResponse CreateToken(AppUser user, CancellationToken cancellationToken = default)
    {
        var now = _clock.Current();
        var expires = now.Add(_expiry);

        var token = GenerateJwt(user, expires);
        var refreshToken = GenerateRefreshToken();
        return new TokenResponse(token, refreshToken, expires);
    }
    
    
   

    /*public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
       
        var refreshTokenProtector = _bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(request.RefreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            DateTime.UtcNow >= expiresUtc ||
            await _signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not AppUser user)

        {
            return await Result<TokenResponse>.FailAsync("Invalid token");
        }

        var newPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        return await GenerateTokensAndUpdateUser(user);
    }*/
    
    private string GenerateJwt(AppUser user, DateTime expires) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user), expires);
    
    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims, DateTime expires)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            audience: _jwtSettings.Audience,
            issuer: _jwtSettings.Issuer,
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    
    private Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey)),
            ValidateIssuer = !string.IsNullOrWhiteSpace(_jwtSettings.Issuer),
            ValidateAudience = !string.IsNullOrWhiteSpace(_jwtSettings.Audience),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            return Result<ClaimsPrincipal>.Fail("Invalid Token.");
        }

        return principal;
    }
}