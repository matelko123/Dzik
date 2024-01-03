using Application.Common.Interfaces;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Shared.Wrapper;

namespace Application.Identity.Auth;

public interface IAuthenticationService : ITransientService
{
    Task<Result<Guid>> RegisterUserAsync(AppUser user, string password, CancellationToken cancellationToken = default);

    Task<Result<TokenResponse>> LoginAsync(string email, string password, bool isPersistent, CancellationToken cancellationToken = default);
}