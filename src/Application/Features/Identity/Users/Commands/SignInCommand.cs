using Application.Abstractions.Messaging;
using Application.Errors;
using Application.Identity.Tokens;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Wrapper;

namespace Application.Features.Identity.Users.Commands;

public sealed record SignInCommand(string Email, string Password) : ICommand<Result<TokenResponse>>;

public sealed class SignInCommandHandler : ICommandHandler<SignInCommand, Result<TokenResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenStorage _tokenStorage;
    private readonly ITokenService _tokenService;

    public SignInCommandHandler(
        UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager, 
        ITokenStorage tokenStorage, 
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenStorage = tokenStorage;
        _tokenService = tokenService;
    }

    public async Task<Result<TokenResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return await Result<TokenResponse>.FailAsync(UserErrors.NotFound);
        }
        
        // If user confirm Email if is required

        SignInResult isPasswordValid = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);
        if (!isPasswordValid.Succeeded)
        {
            if (isPasswordValid.IsLockedOut)
            {
                return await Result<TokenResponse>.FailAsync(UserErrors.LockedOut);
            }
            
            return await Result<TokenResponse>.FailAsync(UserErrors.NotAllowed);
        }
        
        // Generate JWT & refresh
        Result<TokenResponse> token = await _tokenService.CreateTokenAsync(user, cancellationToken);
        if (!token.Succeeded)
        {
            return await Result<TokenResponse>.FailAsync(token.Messages);
        }
        
        // Set JWT to httpContext
        _tokenStorage.Set(token.Data!.Token);
        return token;
    }
}