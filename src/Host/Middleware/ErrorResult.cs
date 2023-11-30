namespace Host.Middleware;

public class ErrorResult
{
    public string? Exception { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; set; }
}