using Application.Common.Interfaces;
using Domain.Entities.Identity;
using Shared.Wrapper;

namespace Application.Identity.Tokens;

public interface IRefreshTokenService : ITransientService
{
    Task<Result<string>> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<AppRefreshToken?>> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AppRefreshToken>>> GetRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ValidateAsync(string token, CancellationToken cancellationToken = default);
}