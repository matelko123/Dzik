using Application.Identity.Roles;
using Contracts.Common;
using Contracts.Identity.Roles;
using Domain.Entities.Identity;
using Host.Endpoints.Internal;
using Host.Extensions;
using Shared.Wrapper;

namespace Host.Endpoints.Identity;

public class RolesEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        var rolesGroup = app
            .MapGroup("roles")
            .WithTags("Roles");

        rolesGroup.MapGet("/", async (
            IRoleService roleService, CancellationToken cancellationToken) =>
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


        rolesGroup.MapGet("/{id:guid}", async (
            Guid id,
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var role = await roleService.GetByIdWithPermissionsAsync(id, cancellationToken);
            return role.ToApiResult();
        })
            .WithName("GetRoleById")
            .Produces<Result<RoleDto>>()
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);


        rolesGroup.MapPost("/", async (
            CreateRoleRequest request,
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var role = new AppRole(request.Name, request.Description);
            var result = await roleService.CreateAsync(role, cancellationToken);
            return result.ToApiResult();
        })
            .WithName("CreateRole")
            .Accepts<CreateRoleRequest>(EndpointConstants.ContentType)
            .Produces<Result>(StatusCodes.Status201Created)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);


        rolesGroup.MapDelete("/{id:guid}", async (
            Guid id,
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var result = await roleService.DeleteAsync(id, cancellationToken);
            return result.ToApiResult();
        })
            .WithName("DeleteRole")
            .Produces<Result>()
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);


        rolesGroup.MapPut("/{id:guid}", async (
            Guid id, UpdateRoleRequest request,
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var roles = await roleService.UpdateAsync(request with { RoleId = id }, cancellationToken);
            return roles.ToApiResult();
        })
            .WithName("UpdateRole")
            .Produces<Result>()
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
