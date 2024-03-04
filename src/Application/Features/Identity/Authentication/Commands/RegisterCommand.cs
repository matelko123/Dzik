using Application.Abstractions.Messaging;
using Application.Errors;
using Application.Identity.Auth;
using Application.Identity.Users;
using Domain.Entities.Identity;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Wrapper;

namespace Application.Features.Identity.Authentication.Commands;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string PhoneNumber
    ) : ICommand<Result<Guid>>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IUserService userService, IOptions<IdentityOptions> options)
    {
        var identityOptions = options.Value;
        
        RuleFor(u => u.FirstName)
            .NotEmpty();
        
        RuleFor(u => u.LastName)
            .NotEmpty();
        
        RuleFor(u => u.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, _) => !await userService.ExistsWithEmailAsync(email))
                .WithMessage(UserErrors.Validation.EmailAlreadyTaken);

        RuleFor(u => u.UserName).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(6)
            .MustAsync(async (name, _) => !await userService.ExistsWithNameAsync(name))
                .WithMessage(UserErrors.Validation.UsernameAlreadyTaken);


        RuleFor(u => u.Password).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(identityOptions.Password.RequiredLength);
        
        RuleFor(x => x.PhoneNumber).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage(UserErrors.Validation.PhoneNumberInvalidFormat)
            .MustAsync(async (phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!))
                .WithMessage(UserErrors.Validation.PhoneNumberAlreadyTaken);
    }
}

internal sealed class RegisterCommandHandler(
    IAuthenticationService authenticationService)
    : ICommandHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        AppUser user = request.Adapt<AppUser>();
        Result<Guid> result = await authenticationService.RegisterUserAsync(user, request.Password, cancellationToken);
        return result;
    }
}