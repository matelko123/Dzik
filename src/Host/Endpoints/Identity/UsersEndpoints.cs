using Application.Features.Identity.Users.Commands;
using Host.Endpoints.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Host.Endpoints.Identity;

public class UsersEndpoints : IEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Users";
    private const string BaseRoute = "identity/users";
    
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, async (
            [FromBody] CreateUserCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var client = await sender.Send(command, cancellationToken);
            return Results.Ok(client);
        })
            .WithName("CreateClient")
            .WithTags(Tag)
            .Accepts<CreateUserCommand>(ContentType)
            .Produces<Guid>(StatusCodes.Status201Created);
    }
}