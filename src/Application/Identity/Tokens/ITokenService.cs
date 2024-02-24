using Application.Common.Interfaces;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Shared.Wrapper;

namespace Application.Identity.Tokens;

public interface ITokenService
{
    Task<Result<TokenResponse>> CreateTokenAsync(AppUser user, CancellationToken cancellationToken = default);    
    Task<Result<TokenResponse>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
}