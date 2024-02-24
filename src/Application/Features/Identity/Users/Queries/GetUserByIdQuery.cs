using Application.Abstractions.Messaging;
using Application.Errors;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Wrapper;

namespace Application.Features.Identity.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : ICachedQuery<Result<AppUser>>
{
    public string Key => $"users-by-id-{UserId}";
    public TimeSpan? Expiration => null;
}

internal sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<AppUser>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetUserByIdQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<AppUser>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByIdAsync(request.UserId.ToString());
        return user is null 
            ? Result<AppUser>.Error(UserErrors.NotFound) 
            : Result<AppUser>.Success(user);
    }
}