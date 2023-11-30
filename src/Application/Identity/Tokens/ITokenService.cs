using Application.Common.Interfaces;
using Domain.Entities.Identity;

namespace Application.Identity.Tokens;

public interface ITokenService : ITransientService
{
    TokenResponse CreateToken(AppUser user, CancellationToken cancellationToken = default);
}