using Application.Errors;
using Application.Identity.Tokens;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Wrapper;
using IAuthenticationService = Application.Identity.Auth.IAuthenticationService;

namespace Infrastructure.Auth;

public class AuthenticationService : SignInManager<AppUser>, IAuthenticationService
{
    private readonly ITokenService _tokenService;
    
    public AuthenticationService(UserManager<AppUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<AppUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<AppUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<AppUser> confirmation, 
        ITokenService tokenService)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _tokenService = tokenService;
    }
    
    public async Task<Result<Guid>> RegisterUserAsync(AppUser user, string password, CancellationToken cancellationToken = default)
    {
            var result = await UserManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return Result<Guid>.Error(result.GetErrors());
            }
        
            // TODO: Send confirmation email if is set
            // if (_securitySettings.RequireConfirmedAccount && !string.IsNullOrEmpty(user.Email))

            return user.Id;
    }

    public async Task<Result<TokenResponse>> LoginAsync(string email, string password, bool isPersistent, CancellationToken cancellationToken = default)
    {
        AppUser? user = await UserManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result<TokenResponse>.Error(UserErrors.NotFound);
        }
        
        // If user confirm Email if is required
        SignInResult isPasswordValid = await PasswordSignInAsync(user, password, isPersistent, false);
        if (!isPasswordValid.Succeeded)
        {
            if (isPasswordValid.IsLockedOut)
            {
                return Result<TokenResponse>.Error(UserErrors.LockedOut);
            }
            
            return Result<TokenResponse>.Error(UserErrors.NotAllowed);
        }
        
        // Generate JWT & refresh
        Result<TokenResponse> token = await _tokenService.CreateTokenAsync(user, cancellationToken);
        if (!token.IsSuccess)
        {
            return token;
        }
        
        return token;
    }
}