using System.Security.Claims;
using Application.Features.Identity.Authentication.Commands;
using Application.Features.Identity.Tokens.Commands;
using Application.Features.Identity.Users.Queries;
using Contracts.Identity.Authentication;
using Domain.Entities.Identity;
using Host.Endpoints.Internal;
using Host.Middleware;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
using Shared.Wrapper;

namespace Host.Endpoints.Identity;

public class AuthEndpoints : IEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Auth";
    private const string BaseRoute = "auth";

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost($"{BaseRoute}/register", async (
                [FromBody] RegisterRequest request,
                ISender sender, CancellationToken cancellationToken) =>
            {
                RegisterCommand command = request.Adapt<RegisterCommand>();
                Result<Guid> client = await sender.Send(command, cancellationToken);
                return client.Match(
                    Results.Ok,
                    Results.BadRequest);
            })
            .WithName("Register")
            .WithTags(Tag)
            .Accepts<RegisterRequest>(ContentType)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ErrorResult>(StatusCodes.Status422UnprocessableEntity);

        app.MapPost($"{BaseRoute}/login", async (
                [FromBody] LoginRequest request,
                ISender sender, CancellationToken cancellationToken) =>
            {
                LoginCommand command = request.Adapt<LoginCommand>();
                Result<TokenResponse> client = await sender.Send(command, cancellationToken);
                return client.Match(
                    Results.Ok,
                    Results.BadRequest);
            })
            .WithName("Login")
            .WithTags(Tag)
            .Accepts<LoginRequest>(ContentType)
            .Produces<TokenResponse>();


        app.MapPost($"{BaseRoute}/refresh", async (
                [FromBody] RefreshTokenCommand request,
                ISender sender, CancellationToken cancellationToken) =>
            {
                Result<TokenResponse> result = await sender.Send(request, cancellationToken);
                return result.Match(
                    Results.Ok,
                    Results.BadRequest);
            })
            .WithName("Refresh token")
            .WithTags(Tag)
            .Accepts<LoginCommand>(ContentType)
            .Produces<TokenResponse>()
            .RequireAuthorization();

        app.MapPost($"{BaseRoute}/info", (
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