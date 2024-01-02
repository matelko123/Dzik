using Application.Abstractions.Messaging;
using Application.Errors;
using Application.Identity.Users;
using FluentValidation;
using Shared.Wrapper;

namespace Application.Features.Identity.Users.Commands;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string? PhoneNumber
    ) : ICommand<Result<Guid>>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IUserService userService)
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

internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(request, cancellationToken);
        return result;
    }
}