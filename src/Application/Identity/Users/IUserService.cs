using Application.Common.Interfaces;
using Application.Features.Identity.Users.Commands;

namespace Application.Identity.Users;

public interface IUserService : ITransientService
{
    Task<Guid> CreateAsync(CreateUserCommand request, CancellationToken token = default);
}