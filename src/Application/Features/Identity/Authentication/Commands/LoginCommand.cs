using Application.Abstractions.Messaging;
using Application.Identity.Auth;
using Contracts.Identity.Authentication;
using FluentValidation;
using Shared.Wrapper;

namespace Application.Features.Identity.Authentication.Commands;

public sealed record LoginCommand(
    string Email, 
    string Password, 
    bool IsPersistent,
    string TwoFactorCode,
    string TwoFactorRecoveryCode
    ) : ICommand<Result<TokenResponse>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}

internal sealed class LoginCommandHandler(
    IAuthenticationService authenticationService)
    : ICommandHandler<LoginCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Result<TokenResponse> result = await authenticationService.LoginAsync(request, cancellationToken);
        
        return result;
    }
}