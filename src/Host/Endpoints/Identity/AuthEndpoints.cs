using System.Security.Claims;
using Application.Features.Identity.Users.Commands;
using Application.Identity.Tokens;
using Host.Endpoints.Internal;
using Host.Middleware;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;

namespace Host.Endpoints.Identity;

public class AuthEndpoints : IEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Auth";
    private const string BaseRoute = "auth";
    
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost($"{BaseRoute}/signup", async (
                [FromBody] CreateUserCommand command, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var client = await sender.Send(command, cancellationToken);
                return client.Match(
                    Results.Ok,
                    Results.BadRequest);
            })
            .WithName("Sign up")
            .WithTags(Tag)
            .Accepts<CreateUserCommand>(ContentType)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ErrorResult>(StatusCodes.Status422UnprocessableEntity);
        
        app.MapPost($"{BaseRoute}/signin", async (
                [FromBody] SignInCommand command, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var client = await sender.Send(command, cancellationToken);
                return client.Match(
                    Results.Ok,
                    Results.BadRequest);
            })
            .WithName("Sign in")
            .WithTags(Tag)
            .Accepts<SignInCommand>(ContentType)
            .Produces<TokenResponse>();
        
        
        app.MapPost($"{BaseRoute}/refresh", async (
                [FromBody] RefreshTokenCommand request, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);
                return result.Match(
                    Results.Ok,
                    Results.BadRequest);
            })
            .WithName("Refresh token")
            .WithTags(Tag)
            .Accepts<SignInCommand>(ContentType)
            .Produces<TokenResponse>()
            .RequireAuthorization();
        
        app.MapPost($"{BaseRoute}/info",  (
                ClaimsPrincipal claims) =>
            {
                var result = new
                {
                    Id = claims.GetUserId(),
                    FirstName = claims.GetFirstName(),
                    LastName = claims.GetLastName(),
                    Email = claims.GetEmail(),
                    PhoneNumber = claims.GetPhoneNumber(),
                    Claims = claims.Claims.Select(x => x.Value).ToArray()
                };
                
                return Results.Ok(result);
            })
            .WithName("Info")
            .WithTags(Tag)
            .RequireAuthorization();
    }
}