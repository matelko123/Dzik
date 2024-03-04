using Application.Errors;
using Application.Features.Identity.Authentication.Commands;
using Application.Identity.Auth;
using Application.Identity.Tokens;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Authorization.Constants.Role;
using Shared.Wrapper;

namespace Infrastructure.Auth;

public class AuthenticationService(
    ITokenService tokenService,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IOptions<IdentityOptions> identityOptions)
    : IAuthenticationService
{
    private readonly IdentityOptions _identityOptions = identityOptions.Value;

    public async Task<Result<Guid>> RegisterUserAsync(AppUser user, string password, CancellationToken cancellationToken = default)
    {
        IdentityResult result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return Result<Guid>.Error(result.GetErrors());
        }

        await userManager.AddToRoleAsync(user, RoleConstants.BasicRole);

        // TODO: Send confirmation email if is set
        // if (_identityOptions.SignIn.RequireConfirmedEmail && !string.IsNullOrEmpty(user.Email))

        return Result<Guid>.Created(user.Id);
    }

    public async Task<Result<TokenResponse>> LoginAsync(LoginCommand request, CancellationToken cancellationToken = default)
    {
        AppUser? user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Result<TokenResponse>.Error(UserErrors.NotFound);
        }

        if (_identityOptions.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            return Result<TokenResponse>.Error(UserErrors.EmailNotConfirmed);
        }

        SignInResult signInResult = await signInManager.PasswordSignInAsync(user, request.Password, request.IsPersistent, false);
        
        if (signInResult.IsLockedOut)
        {
            return Result<TokenResponse>.Error(UserErrors.LockedOut);
        }

        if (signInResult.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(request.TwoFactorCode))
            {
                signInResult = await signInManager.TwoFactorAuthenticatorSignInAsync(request.TwoFactorCode, request.IsPersistent, rememberClient: request.IsPersistent);
            }
            else if (!string.IsNullOrEmpty(request.TwoFactorRecoveryCode))
            {
                signInResult = await signInManager.TwoFactorRecoveryCodeSignInAsync(request.TwoFactorRecoveryCode);
            }
        }

        if (!signInResult.Succeeded)
        {
            return Result<TokenResponse>.Error(UserErrors.WrongPassword);
        }
        
        // Generate JWT & refresh
        Result<TokenResponse> token = await tokenService.CreateTokenAsync(user, cancellationToken);
        return token;
    }
}