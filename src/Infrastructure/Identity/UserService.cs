using Application.Features.Identity.Users.Commands;
using Application.Identity.Users;
using Domain.Entities.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;

    public UserService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Guid> CreateAsync(CreateUserCommand request, CancellationToken token = default)
    {
        AppUser user = request.Adapt<AppUser>();
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new Exception($"Validation Errors Occurred. {result.GetErrors()}");
        }
        
        // TODO: Send confirmation email if is set
        // if (_securitySettings.RequireConfirmedAccount && !string.IsNullOrEmpty(user.Email))

        return user.Id;
    }
}