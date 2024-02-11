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

public class TokenService : ITokenService
{
    private readonly IClock _clock;
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IRefreshTokenService _refreshTokenService;

    private readonly TimeSpan _expiry;

    public TokenService(
        IClock clock,
        IOptions<JwtSettings> jwtSettings,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IRefreshTokenService refreshTokenService)
    {
        _clock = clock;
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;

        _expiry = _jwtSettings.TokenExpiration ?? TimeSpan.FromHours(1);
    }

    public async Task<Result<TokenResponse>> CreateTokenAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        DateTime now = _clock.Current();
        DateTime expires = now.Add(_expiry);

        string token = await GenerateJwt(user, expires);

        Result<string> refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        if (!refreshToken)
        {
            return await Result<TokenResponse>.FailAsync(refreshToken);
        }

        return new TokenResponse(token, refreshToken.Data!, expires);
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        Result<ClaimsPrincipal> userPrincipal = GetPrincipalFromExpiredToken(token);
        if (!userPrincipal.Succeeded)
        {
            return await Result<TokenResponse>.FailAsync(userPrincipal.Messages);
        }

        string? userEmail = userPrincipal.Data?.GetEmail();

        AppUser? user = await _userManager.FindByEmailAsync(userEmail!);
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

    private async Task<string> GenerateJwt(AppUser user, DateTime expires) =>
        GenerateEncryptedToken(GetSigningCredentials(), await GetClaims(user), expires);

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims, DateTime expires)
    {
        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            audience: _jwtSettings.Audience,
            issuer: _jwtSettings.Issuer,
            signingCredentials: signingCredentials);
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey)),
            ValidateIssuer = !string.IsNullOrWhiteSpace(_jwtSettings.Issuer),
            ValidateAudience = !string.IsNullOrWhiteSpace(_jwtSettings.Audience),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken? securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            return Result<ClaimsPrincipal>.Fail("Invalid Token.");
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> GetClaims(AppUser user)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        IList<string> roles = await _userManager.GetRolesAsync(user);
        List<Claim> roleClaims = new();
        List<Claim> permissionClaims = new();

        foreach (string role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            AppRole? thisRole = await _roleManager.FindByNameAsync(role);
            if (thisRole == null) continue;

            IList<Claim> allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
            permissionClaims.AddRange(allPermissionsForThisRoles);
        }

        IEnumerable<Claim> customUserClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };

        return customUserClaims
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);
    }
}