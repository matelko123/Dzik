using Application.Features.Identity.Tokens.Commands;
using Application.Identity.Tokens;
using Contracts.Identity.Authentication;
using Host.Endpoints.Internal;
using Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrapper;

namespace Host.Endpoints.Identity;

public class TokenEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder tokenGroup = app
            .MapGroup("api/v1/token")
            .WithTags("Tokens");

        tokenGroup.MapPost("/refresh", async (
                [FromBody] string refreshToken,
                ISender sender, ITokenStorage tokenStorage, CancellationToken cancellationToken) =>
            {
                string token = tokenStorage.Get()!;
                RefreshTokenCommand command = new(token, refreshToken);
                Result<TokenResponse> result = await sender.Send(command, cancellationToken);
                return result.ToMinimalApiResult();
            })
            .WithName("Refresh token")
            .Accepts<string>(EndpointConstants.ContentType)
            .Produces<Result<TokenResponse>>()
            .RequireAuthorization();
    }
}