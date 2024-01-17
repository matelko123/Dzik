using Application.Common.Interfaces;
using Domain.Entities.Identity;
using Shared.Wrapper;

namespace Application.Identity.Tokens;

public interface IRefreshTokenService : ITransientService
{
    /// <summary>
    /// Create new refresh token.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Refresh token</returns>
    Task<Result<string>> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
   
    /// <summary>
    /// Get token entity by token value
    /// </summary>
    /// <param name="token">Token</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Refresh token entity</returns>
    Task<Result<AppRefreshToken?>> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get list of user's tokens.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of user's tokens</returns>
    Task<Result<IReadOnlyCollection<AppRefreshToken>>> GetRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if token is valid.
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Result with Succeeded flag</returns>
    Task<Result> ValidateAsync(string token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Revoke all active user tokens.
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken"></param>
    /// </summary>
    Task RevokeAllTokensForUser(Guid userId, CancellationToken cancellationToken = default);
}