using Application.Features.Identity.Users.Queries;
using Contracts.Identity.Authentication;
using Host.Endpoints.Internal;
using Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrapper;

namespace Host.Endpoints.Identity;

public class UserEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        var userGroup = app.MapGroup("api/users")
            .WithTags("Users")
            .RequireAuthorization();

        userGroup.MapGet($"{{userId:guid}}", async (
                [FromRoute] Guid userId,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetCachedUser")
            .Produces<Result<UserDto>>();
    }
}
