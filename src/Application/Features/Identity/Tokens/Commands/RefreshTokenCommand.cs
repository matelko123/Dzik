using Application.Abstractions.Messaging;
using Application.Identity.Tokens;
using Contracts.Identity.Authentication;
using FluentValidation;
using Shared.Wrapper;

namespace Application.Features.Identity.Tokens.Commands;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<Result<TokenResponse>>;

internal sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}

internal sealed class RefreshTokenCommandHandler(
    ITokenService tokenService)
    : ICommandHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Result<TokenResponse> result = await tokenService.RefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result;
    }
}