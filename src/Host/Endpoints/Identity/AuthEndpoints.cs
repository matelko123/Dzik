using Application.Features.Identity.Authentication.Commands;
using Contracts.Identity.Authentication;
using Host.Endpoints.Internal;
using Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
using Shared.Wrapper;
using System.Security.Claims;

namespace Host.Endpoints.Identity;

public class AuthEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder authGroup = app
            .MapGroup("api/auth")
            .WithTags("Authorization");

        authGroup.MapPost("/register", async (
                [FromBody] RegisterRequest request,
                [FromServices] ISender sender, CancellationToken cancellationToken) =>
            {
                RegisterCommand command = new(request.FirstName, request.LastName, request.UserName, request.Email,
                    request.Password, request.PhoneNumber);
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result
                    ? Results.CreatedAtRoute("GetCachedUser", new { userId = result.Value }, result)
                    : result.ToApiResult();
            })
            .WithName("Register")
            .Accepts<RegisterRequest>(EndpointConstants.ContentType)
            .Produces<Result<Guid>>(StatusCodes.Status201Created);


        authGroup.MapPost("/signin", async (
                [FromBody] LoginRequest request,
                [FromServices] ISender sender, CancellationToken cancellationToken) =>
            {
                LoginCommand command = new(request.Email, request.Password, request.IsPersistent, request.TwoFactorCode, request.TwoFactorRecoveryCode);
                Result<TokenResponse> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult();
            })
            .WithName("Login")
            .Accepts<LoginRequest>(EndpointConstants.ContentType)
            .Produces<Result<TokenResponse>>();

        authGroup.MapPost("/me", (
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
            .WithName("Me")
            .RequireAuthorization();
    }
}