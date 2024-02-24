using Application.Abstractions.Messaging;
using Application.Identity.Auth;
using Application.Identity.Tokens;
using Contracts.Identity.Authentication;
using Shared.Wrapper;

namespace Application.Features.Identity.Authentication.Commands;

public sealed record LoginCommand(string Email, string Password, bool IsPersistent) : ICommand<Result<TokenResponse>>;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenStorage _tokenStorage;

    public LoginCommandHandler(
        ITokenStorage tokenStorage,
        IAuthenticationService authenticationService)
    {
        _tokenStorage = tokenStorage;
        _authenticationService = authenticationService;
    }

    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Result<TokenResponse> result = await _authenticationService.LoginAsync(request.Email, request.Password, request.IsPersistent, cancellationToken);

        if (result)
        {
            // Set JWT to httpContext
            _tokenStorage.Set(result.Value!.Token);
        }
        
        return result;
    }
}