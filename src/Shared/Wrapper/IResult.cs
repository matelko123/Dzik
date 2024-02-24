using Contracts.Common;
using System.Net;

namespace Shared.Wrapper;

public interface IResult
{
    HttpStatusCode Status { get; }
    IEnumerable<string> Errors { get; }
    bool IsSuccess { get; }
}


public interface IResult<T> : IResult
{
    T? Value { get; }
    Type ValueType { get; }
}