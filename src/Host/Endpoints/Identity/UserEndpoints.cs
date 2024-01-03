using Application.Features.Identity.Users.Queries;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Host.Endpoints.Internal;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrapper;

namespace Host.Endpoints.Identity;

public class UserEndpoints : IEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Users";
    private const string BaseRoute = "users";

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseRoute}/{{userId:guid}}", async (
                [FromRoute] Guid userId,
                ISender sender, CancellationToken cancellationToken) =>
            {
                Result<AppUser> result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);
                return result
                    ? Results.Ok(result.Data.Adapt<UserDto>())
                    : Results.BadRequest(result.Messages);
            })
            .WithName("Get cached user")
            .WithTags(Tag)
            .Produces<UserDto>();
    }
}
