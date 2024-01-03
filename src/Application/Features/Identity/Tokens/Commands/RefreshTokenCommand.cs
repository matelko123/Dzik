using Application.Abstractions.Messaging;
using Application.Identity.Tokens;
using Contracts.Identity.Authentication;
using Shared.Wrapper;

namespace Application.Features.Identity.Tokens.Commands;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<Result<TokenResponse>>;

internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Result<TokenResponse> result = await _tokenService.RefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result;
    }
}