using System.Security.Cryptography;
using Application.Common.Interfaces;
using Application.Identity.Tokens;
using Domain.Entities.Identity;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Wrapper;

namespace Infrastructure.Identity;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IClock _clock;
    private readonly AppDbContext _dbContext;

    public RefreshTokenService(AppDbContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<Result<string>> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        string token = GenerateRefreshToken();
        DateTime currentDate = _clock.Current();
        
        // Create new refresh token
        AppRefreshToken refreshToken = new()
        {
            Token = token,
            UserId = userId,
            CreatedDate = currentDate,
            ExpiryDate = currentDate.AddDays(7),
        };
        
        // Revoke all user's tokens
        await RevokeAllTokensForUser(userId, cancellationToken);

        // Save changes
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return token;
    }


    public async Task<Result<AppRefreshToken?>> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task<Result<IReadOnlyCollection<AppRefreshToken>>> GetRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .Where(x => x.UserId == userId)
            .Where(x => !x.IsRevoked)
            .OrderByDescending(x => x.CreatedDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Result> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        DateTime currentDate = _clock.Current();
        Result<AppRefreshToken?>? refreshToken = await GetRefreshTokenAsync(token, cancellationToken);

        return refreshToken?.Data is not null && !refreshToken.Data.IsRevoked && refreshToken.Data.ExpiryDate > currentDate;
    }

    public async Task RevokeAllTokensForUser(Guid userId, CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens
            .Where(x => x.UserId == userId)
            .Where(x => !x.IsRevoked)
            .ExecuteUpdateAsync(t =>
                t.SetProperty(b => b.IsRevoked, b => true), cancellationToken);
    }
    
        
    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}