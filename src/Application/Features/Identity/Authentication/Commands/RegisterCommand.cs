using Application.Abstractions.Messaging;
using Application.Errors;
using Application.Identity.Auth;
using Application.Identity.Users;
using Domain.Entities.Identity;
using FluentValidation;
using Mapster;
using Shared.Wrapper;

namespace Application.Features.Identity.Authentication.Commands;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string? PhoneNumber
    ) : ICommand<Result<Guid>>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IUserService userService)
    {
        RuleFor(u => u.FirstName)
            .MinimumLength(3);
        
        RuleFor(u => u.LastName)
            .MinimumLength(5);
        
        RuleFor(u => u.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
                .WithMessage(UserErrors.Validation.InvalidEmail)
            .MustAsync(async (email, _) => !await userService.ExistsWithEmailAsync(email))
                .WithMessage((_, email) => UserErrors.Validation.EmailAlreadyTaken(email));

        RuleFor(u => u.UserName).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(20)
            .MustAsync(async (name, _) => !await userService.ExistsWithNameAsync(name))
                .WithMessage((_, name) => UserErrors.Validation.UsernameAlreadyTaken(name));
        
        RuleFor(x => x.PhoneNumber).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(UserErrors.Validation.InvalidPhoneNumberFormat)
            .MustAsync(async (phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!))
                .WithMessage((_, phone) => UserErrors.Validation.PhoneNumberAlreadyTaken(phone));
    }
}

internal sealed class RegisterCommandHandler
    : ICommandHandler<RegisterCommand, Result<Guid>>
{
    private readonly IAuthenticationService _authenticationService;

    public RegisterCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        AppUser user = request.Adapt<AppUser>();
        Result<Guid> result = await _authenticationService.RegisterUserAsync(user, request.Password, cancellationToken);
        return result;
    }
}