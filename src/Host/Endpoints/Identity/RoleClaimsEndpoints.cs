using Application.Identity.RoleClaim;
using Contracts.Identity.RoleClaims;
using Host.Endpoints.Internal;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrapper;

namespace Host.Endpoints.Identity;

public class RoleClaimsEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        var roleClaimsGroup = app
            .MapGroup("api/identity/roleclaims")
            .WithTags("RoleClaims");

        roleClaimsGroup.MapGet("/", async (
            [FromServices] IRoleClaimService roleClaimService, CancellationToken cancellationToken) =>
        {
            var roles = await roleClaimService.GetAllAsync(cancellationToken);
            return TypedResults.Ok(roles);
        })
            .WithName("GetAllRoleClaims")
            .WithOpenApi(operation =>
            {
                operation.Description = "Get all claims";
                operation.Summary = "Return all permissions";
                return operation;
            })
            .Produces<Result<RoleClaimDto>>();

        roleClaimsGroup.MapGet("/{id:guid}", async (
            [FromRoute] Guid id,
            [FromServices] IRoleClaimService roleClaimService, CancellationToken cancellationToken) =>
        {
            var roles = await roleClaimService.GetAllByRoleIdAsync(id, cancellationToken);
            return TypedResults.Ok(roles);
        })
            .WithName("GetRoleClaimByRoleId")
            .WithOpenApi(operation =>
            {
                operation.Description = "Get claims by role id";
                operation.Summary = "Return all permissions";
                return operation;
            })
            .Produces<Result<RoleClaimDto>>();

        roleClaimsGroup.MapPut("/{id:guid}", async (
            [FromRoute] Guid id, [FromBody] RoleClaimRequest request,
            [FromServices] IRoleClaimService roleClaimService, CancellationToken cancellationToken) =>
        {
            var roles = await roleClaimService.UpdateAsync(request with { RoleId = id }, cancellationToken);
            return TypedResults.Ok();
        })
            .WithName("UpdateRoleClaims")
            .WithOpenApi(operation =>
            {
                operation.Description = "Get claim by id";
                operation.Summary = "Return all permissions";
                return operation;
            })
            .Produces<Result>();
    }
}
