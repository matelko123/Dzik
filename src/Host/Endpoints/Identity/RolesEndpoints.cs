﻿using Application.Identity.Roles;
using Contracts.Identity.Roles;
using Domain.Entities.Identity;
using Host.Endpoints.Internal;
using Host.Middleware;
using Mapster;
using Shared.Wrapper;

namespace Host.Endpoints.Identity;

public class RolesEndpoints : IEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Roles";
    private const string BaseRoute = "roles";


    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        var rolesGroup = app.MapGroup(BaseRoute);

        rolesGroup.MapGet("/", async (
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var roles = await roleService.GetListAsync(cancellationToken);
            return Results.Ok(roles.Data!.Adapt<List<RoleDto>>());
        })
            .WithName("GetAllRoles")
            .WithTags(Tag)
            .WithOpenApi(operation =>
            {
                operation.Description = "test";
                operation.Summary = "Return all roles";
                return operation;
            })
            .Produces<List<RoleDto>>(StatusCodes.Status200OK);


        rolesGroup.MapGet("/{id:guid}", async (
            Guid id,
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var role = await roleService.GetByIdWithPermissionsAsync(id, cancellationToken);
            return role.Match(Results.Ok, Results.NotFound);
        })
            .WithName("GetRoleById")
            .WithTags(Tag)
            .Produces<Result<RoleDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound);


        rolesGroup.MapPost("/", async (
            CreateRoleRequest request,
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var role = new AppRole(request.Name, request.Description);
            var roles = await roleService.CreateAsync(role, cancellationToken);
            return roles.Match(
                Results.Created,
                Results.BadRequest);
        })
            .WithName("CreateRole")
            .WithTags(Tag)
            .Accepts<CreateRoleRequest>(ContentType)
            .Produces<Result>(StatusCodes.Status201Created)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest);


        rolesGroup.MapDelete("/{id:guid}", async (
            Guid id,
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var result = await roleService.DeleteAsync(id, cancellationToken);
            return result.Match(
                Results.NoContent,
                Results.NotFound);
        })
            .WithName("DeleteRole")
            .WithTags(Tag)
            .Produces<Result>(StatusCodes.Status204NoContent)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest);


        rolesGroup.MapPut("/{id:guid}", async (
            Guid id, UpdateRoleRequest request,
            IRoleService roleService, CancellationToken cancellationToken) =>
        {
            var roles = await roleService.UpdateAsync(request with { RoleId = id }, cancellationToken);
            return roles.Match(
                Results.NoContent,
                Results.NotFound);
        })
            .WithName("UpdateRole")
            .WithTags(Tag)
            .Produces<Result>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest);
    }
}