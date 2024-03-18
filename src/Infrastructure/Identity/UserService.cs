using Application.Errors;
using Application.Identity.Users;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Wrapper;

namespace Infrastructure.Identity;

public class UserService(
    UserManager<AppUser> userManager, 
    AppDbContext appDbContext) 
    : IUserService
{

    public async Task<bool> ExistsWithNameAsync(string name)
        => await userManager.FindByNameAsync(name) is not null;

    public async Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null)
        => await userManager.FindByEmailAsync(email) is { } user && user.Id != exceptId;

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null)
        => await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is { } user && user.Id != exceptId;

    public async Task<Result<List<UserDto>>> GetUsersByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await appDbContext.Roles.SingleOrDefaultAsync(x => x.Id == roleId, cancellationToken);
        if (role is null)
        {
            return Result<List<UserDto>>.Error(RoleErrors.NotFound);
        }

        var users = await userManager.GetUsersInRoleAsync(role.Name!);
        return Result<List<UserDto>>.Success(users.Adapt<List<UserDto>>());
    }
}