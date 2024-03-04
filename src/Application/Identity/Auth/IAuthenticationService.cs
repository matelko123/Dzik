using Application.Features.Identity.Authentication.Commands;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Shared.Wrapper;

namespace Application.Identity.Auth;

public interface IAuthenticationService
{
    Task<Result<Guid>> RegisterUserAsync(AppUser request, string password, CancellationToken cancellationToken = default);

    Task<Result<TokenResponse>> LoginAsync(LoginCommand request, CancellationToken cancellationToken = default);
}