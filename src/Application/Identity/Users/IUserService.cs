using Application.Features.Identity.Users.Commands;

namespace Application.Identity.Users;

public interface IUserService
{
    Task<Guid> CreateAsync(CreateUserCommand request, CancellationToken token = default);
}