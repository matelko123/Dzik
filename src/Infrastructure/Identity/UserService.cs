using Application.Features.Identity.Users.Commands;
using Application.Identity.Users;
using Domain.Entities.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;

    public UserService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> ExistsWithNameAsync(string name)
        => await _userManager.FindByNameAsync(name) is not null;

    public async Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null)
        => await _userManager.FindByEmailAsync(email.Normalize()) is AppUser user && user.Id != exceptId;

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null)
        => await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is AppUser user && user.Id != exceptId;

    public async Task<Guid> CreateAsync(CreateUserCommand request, CancellationToken token = default)
    {
        AppUser user = request.Adapt<AppUser>();
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new Exception($"Identity Errors Occurred. {result.GetErrors()}");
        }
        
        // TODO: Send confirmation email if is set
        // if (_securitySettings.RequireConfirmedAccount && !string.IsNullOrEmpty(user.Email))

        return user.Id;
    }
}