using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Application.Identity.Tokens;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Infrastructure.Auth.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Wrapper;
using Shared.Authorization;

namespace Infrastructure.Identity;

public class TokenService: ITokenService
{
    private readonly IClock _clock;
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<AppUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;

    private readonly TimeSpan _expiry;

    public TokenService(
        IClock clock,
        IOptions<JwtSettings> jwtSettings, 
        UserManager<AppUser> userManager, 
        IRefreshTokenService refreshTokenService)
    {
        _clock = clock;
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
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
    
    public async Task<Result<TokenResponse>> CreateTokenAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        var now = _clock.Current();
        var expires = now.Add(_expiry);

        var token = GenerateJwt(user, expires);

        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        return new TokenResponse(token, refreshToken.Data, expires);
    }
    
    public async Task<Result<TokenResponse>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        Result<ClaimsPrincipal> userPrincipal = GetPrincipalFromExpiredToken(token);
        if (!userPrincipal.Succeeded)
        {
            return await Result<TokenResponse>.FailAsync(userPrincipal.Messages);
        }
        
        string? userEmail = userPrincipal.Data.GetEmail();
        
        var user = await _userManager.FindByEmailAsync(userEmail!);
        if (user is null)
        {
            return await Result<TokenResponse>.FailAsync("Authentication Failed.");
        }
        
        if (!await _refreshTokenService.ValidateAsync(refreshToken, cancellationToken))
        {
            return await Result<TokenResponse>.FailAsync("Invalid Refresh Token.");
        }

        return await CreateTokenAsync(user, cancellationToken);
    }
    
    private string GenerateJwt(AppUser user, DateTime expires) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user), expires);
    
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