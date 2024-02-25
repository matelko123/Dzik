using Contracts.Common;
using System.Net;
using System.Text.Json.Serialization;

namespace Shared.Wrapper;

public class Result<T> : Result, IResult<T>
{
    public T? Value { get; }

    public Result(T? value) 
        : base (HttpStatusCode.OK)
    {
        Value = value;
    }

    protected Result(HttpStatusCode status)
        : base (status)
    {
    }

    protected Result(HttpStatusCode status, IEnumerable<string> errors)
        : base(status, errors)
    {
    }

    [JsonIgnore]
    public Type ValueType => typeof(T);

    public static implicit operator T(Result<T> result) => result.Value!;
    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator bool(Result<T> result) => result.IsSuccess;

    public TR Match<TR>(Func<T, TR> success, Func<IEnumerable<string>, TR> fail) =>
        IsSuccess && Value is not null
            ? success(Value)
            : fail(Errors);

    public new static Result<T> Success() => new(HttpStatusCode.OK);
    public static Result<T> Success(T? value) => new(value);
    public new static Result<T> Created() => new(HttpStatusCode.Created);
    public new static Result<T> NoContent() => new(HttpStatusCode.NoContent);
    public new static Result<T> Error(Error error) => new(error.StatusCode, [error.Message]);
    public new static Result<T> Error(params string[] errorMessages) => new(HttpStatusCode.BadRequest, errorMessages);
    public new static Result<T> NotFound() => new(HttpStatusCode.NotFound);
    public new static Result<T> Unauthorized() => new(HttpStatusCode.Unauthorized);
}