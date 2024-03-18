using Application.Identity.Roles;
using Application.Identity.Users;
using Contracts.Common;
using Contracts.Identity.Authentication;
using Contracts.Identity.Roles;
using Domain.Entities.Identity;
using Host.Endpoints.Internal;
using Host.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared.ApiEndpoints;
using Shared.Wrapper;

namespace Host.Endpoints.Identity;

public class RolesEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        var rolesGroup = app
            .MapGroup(RolesEndpointsConstants.Base)
            .WithTags("Roles");

        rolesGroup.MapGet(RolesEndpointsConstants.GetAllRoles, async (
            [FromServices] IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var roles = await roleService.GetListAsync(cancellationToken);
            return roles.ToApiResult();
        })
            .WithName("GetAllRoles")
            .WithOpenApi(operation =>
            {
                operation.Description = "test";
                operation.Summary = "Return all roles";
                return operation;
            })
            .Produces<Result<List<RoleDto>>>();


        rolesGroup.MapGet(RolesEndpointsConstants.GetRoleById, async (
            [FromRoute] Guid id,
            [FromServices] IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var role = await roleService.GetByIdAsync(id, cancellationToken);
            return role.ToApiResult();
        })
            .WithName("GetRoleById")
            .Produces<Result<RoleDto>>()
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);

        rolesGroup.MapGet(RolesEndpointsConstants.GetUsersByRoleId, async (
            [FromRoute] Guid id, [AsParameters] PagedRequest pagedRequest,
            [FromServices] IUserService userService, CancellationToken cancellationToken) =>
        {
            var role = await userService.GetUsersByRoleAsync(id, pagedRequest, cancellationToken);
            return TypedResults.Ok(role);
        })
            .WithName("GetUsersByRoleId")
            .Produces<Result<PagedList<UserDto>>>()
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);

        rolesGroup.MapPost(RolesEndpointsConstants.CreateRole, async (
            [FromBody] RoleDto request,
            [FromServices] IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var role = new AppRole(request.Name, request.Description);
            var result = await roleService.CreateAsync(role, cancellationToken);
            return result.ToApiResult();
        })
            .WithName("CreateRole")
            .Accepts<RoleDto>(EndpointConstants.ContentType)
            .Produces<Result>(StatusCodes.Status201Created)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);


        rolesGroup.MapDelete(RolesEndpointsConstants.DeleteRole, async (
            [FromRoute] Guid id,
            [FromServices] IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var result = await roleService.DeleteAsync(id, cancellationToken);
            return result.ToApiResult();
        })
            .WithName("DeleteRole")
            .Produces<Result>()
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);


        rolesGroup.MapPut(RolesEndpointsConstants.UpdateRole, async (
            [FromRoute] Guid id, [FromBody] RoleDto request,
            [FromServices] IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var roles = await roleService.UpdateAsync(id, request, cancellationToken);
            return roles.ToApiResult();
        })
            .WithName("UpdateRole")
            .Accepts<RoleDto>(EndpointConstants.ContentType)
            .Produces<Result>()
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
