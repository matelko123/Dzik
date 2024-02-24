using Contracts.Common;
using System.Net;

namespace Shared.Wrapper;

public class Result : IResult
{
    public HttpStatusCode Status { get; protected set; } = HttpStatusCode.BadRequest;

    public IEnumerable<string> Errors { get; protected set; } = new List<string>();

    public bool IsSuccess { get; protected set; } = false;

    public Result(HttpStatusCode status, bool isSuccess = false) 
    {
        Status = status;
        IsSuccess = isSuccess;
    }
    public Result(HttpStatusCode status, IEnumerable<string> errors)
    {
        Status = status;
        IsSuccess = false;
        Errors = errors;
    }

    public static implicit operator bool(Result result) => result.Status == HttpStatusCode.OK;
    public static implicit operator Result(bool isSuccess) => new(isSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest);

    public TR Match<TR>(Func<TR> success, Func<IEnumerable<string>, TR> fail) =>
        IsSuccess
            ? success()
            : fail(Errors);

    public static Result Success() => new(HttpStatusCode.OK, true);
    public static Result Created() => new(HttpStatusCode.Created, true);
    public static Result NoContent() => new(HttpStatusCode.NoContent, true);
    public static Result Error(Error error) => new(error.StatusCode, [error.Message]);
    public static Result Error(params string[] errorMessages) => new(HttpStatusCode.BadRequest, errorMessages);
    public static Result NotFound() => new(HttpStatusCode.NotFound, false);
    public static Result Unauthorized() => new(HttpStatusCode.Unauthorized, false);
}
