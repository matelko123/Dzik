using Application.Common.Interfaces;
using Application.Features.Identity.Users.Commands;
using Shared.Wrapper;

namespace Application.Identity.Users;

public interface IUserService : ITransientService
{
    Task<bool> ExistsWithNameAsync(string name);
    Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null);
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null);

    Task<Result<Guid>> CreateAsync(CreateUserCommand request, CancellationToken token = default);
}