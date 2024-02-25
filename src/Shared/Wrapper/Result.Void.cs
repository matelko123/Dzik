using Contracts.Common;
using System.Net;

namespace Shared.Wrapper;

public class Result : IResult
{
    public HttpStatusCode Status { get; init; } = HttpStatusCode.BadRequest;

    public IEnumerable<string> Errors { get; init; } = new List<string>();

    public bool IsSuccess => (int)Status < 300;

    public Result()
    {
    }
    protected internal Result(HttpStatusCode status) 
    {
        Status = status;
    }

    protected internal Result(HttpStatusCode status, IEnumerable<string> errors)
    {
        Status = status;
        Errors = errors;
    }

    public static implicit operator bool(Result result) => result.Status == HttpStatusCode.OK;
    public static implicit operator Result(bool isSuccess) => new(isSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest);

    public TR Match<TR>(Func<TR> success, Func<IEnumerable<string>, TR> fail) =>
        IsSuccess
            ? success()
            : fail(Errors);

    public static Result Success() => new(HttpStatusCode.OK);
    public static Result Created() => new(HttpStatusCode.Created);
    public static Result NoContent() => new(HttpStatusCode.NoContent);
    public static Result Error(Error error) => new(error.StatusCode, [error.Message]);
    public static Result Error(params string[] errorMessages) => new(HttpStatusCode.BadRequest, errorMessages);
    public static Result NotFound() => new(HttpStatusCode.NotFound);
    public static Result Unauthorized() => new(HttpStatusCode.Unauthorized);
}
