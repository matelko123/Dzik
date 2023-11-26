using Application.Abstractions.Messaging;
using Application.Identity.Users;
using FluentValidation;

namespace Application.Features.Identity.Users.Commands;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string? PhoneNumber
    ) : ICommand<Guid>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    // TODO: Add MustAsync for props UserName and Email to be unique
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.FirstName)
            .MinimumLength(3);
        
        RuleFor(u => u.LastName)
            .MinimumLength(5);
        
        RuleFor(u => u.UserName)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(20);
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
    }
}

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(request, cancellationToken);
        return result;
    }
}