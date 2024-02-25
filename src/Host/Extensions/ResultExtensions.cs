using Shared.Wrapper;
using System.Net;

namespace Host.Extensions;

public static class ResultExtensions
{
    public static Microsoft.AspNetCore.Http.IResult ToApiResult<T, TR>(this Result<T> result, Func<T, TR>? func = null)
    {
        return result.ToMinimalApiResult(func);
    }

    public static Microsoft.AspNetCore.Http.IResult ToApiResult<T>(this Result<T> result)
    {
        return result.ToMinimalApiResult<T, T>(null);
    }

    public static Microsoft.AspNetCore.Http.IResult ToApiResult(this Result result)
    {
        return result.ToMinimalApiResult();
    }

    internal static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult<T, TR>(this IResult<T> result, Func<T, TR>? func = null) =>
        result.Status switch
        {
            HttpStatusCode.OK => result.Value is null
                ? Results.Ok()
                : func is null 
                    ? Results.Ok(result)
                    : Results.Ok(Result<TR>.Success(func(result.Value))),
            HttpStatusCode.Created => Results.Created(),
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.NotFound => Results.NotFound(result),
            HttpStatusCode.Unauthorized => Results.Unauthorized(),
            HttpStatusCode.Forbidden => Results.Forbid(),
            HttpStatusCode.BadRequest => Results.BadRequest(result),
            HttpStatusCode.UnprocessableEntity => Results.UnprocessableEntity(result),
            HttpStatusCode.Conflict => Results.Conflict(result),
            HttpStatusCode.ServiceUnavailable => Results.Problem(),
            _ => throw new NotSupportedException($"Result {result.Status} conversion is not supported."),
        };

    internal static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult(this Result result) =>
        result.Status switch
        {
            HttpStatusCode.OK => Results.Ok(result),
            HttpStatusCode.Created => Results.Created(),
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.NotFound => Results.NotFound(result),
            HttpStatusCode.Unauthorized => Results.Unauthorized(),
            HttpStatusCode.Forbidden => Results.Forbid(),
            HttpStatusCode.BadRequest => Results.BadRequest(result),
            HttpStatusCode.UnprocessableEntity => Results.UnprocessableEntity(result),
            HttpStatusCode.Conflict => Results.Conflict(result),
            HttpStatusCode.ServiceUnavailable => Results.Problem(),
            _ => throw new NotSupportedException($"Result {result.Status} conversion is not supported."),
        };
}
