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
            .MinimumLength(UserErrors.Validation.FirstName.MinimumLength)
                .WithMessage(UserErrors.Validation.FirstName.MinimumLengthMessage);
        
        RuleFor(u => u.LastName)
            .MinimumLength(UserErrors.Validation.LastName.MinimumLength)
                .WithMessage(UserErrors.Validation.LastName.MinimumLengthMessage);
        
        RuleFor(u => u.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(UserErrors.Validation.Email.NotEmpty)
            .EmailAddress()
                .WithMessage(UserErrors.Validation.Email.InvalidFormat)
            .MustAsync(async (email, _) => !await userService.ExistsWithEmailAsync(email))
                .WithMessage((_, email) => UserErrors.Validation.Email.AlreadyTaken(email));

        RuleFor(u => u.UserName).Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(UserErrors.Validation.Username.NotEmpty)
            .MinimumLength(UserErrors.Validation.Username.MinimumLength)
                .WithMessage(UserErrors.Validation.Username.MinimumLengthMessage)
            .MustAsync(async (name, _) => !await userService.ExistsWithNameAsync(name))
                .WithMessage((_, name) => UserErrors.Validation.Username.AlreadyTaken(name));


        RuleFor(u => u.Password).Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(UserErrors.Validation.Password.NotEmpty)
            .MinimumLength(identityOptions.Password.RequiredLength)
                .WithMessage((a, b) => UserErrors.Validation.Password.MinimumLengthMessage(identityOptions.Password.RequiredLength));
        
        RuleFor(x => x.PhoneNumber).Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(UserErrors.Validation.PhoneNumber.NotEmpty)
            .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage(UserErrors.Validation.PhoneNumber.InvalidFormat)
            .MustAsync(async (phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!))
                .WithMessage((_, phone) => UserErrors.Validation.PhoneNumber.AlreadyTaken(phone));
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