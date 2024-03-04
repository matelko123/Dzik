using Application.Abstractions.Messaging;
using Application.Errors;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Shared.Wrapper;

namespace Application.Features.Identity.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) 
    : ICachedQuery<Result<UserDto>>
{
    public string Key => $"users-by-id-{UserId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}

internal sealed class GetUserByIdQueryHandler(
    UserManager<AppUser> userManager) 
    : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByIdAsync(request.UserId.ToString());
        return user is null 
            ? Result<UserDto>.Error(UserErrors.NotFound) 
            : Result<UserDto>.Success(user.Adapt<UserDto>());
    }
}