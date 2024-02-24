using System.Net;

namespace Contracts.Common;

public class ErrorResult
{
    public string? Exception { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; set; }
}

public sealed record Error(HttpStatusCode StatusCode, string Message);