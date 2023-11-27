namespace Shared.Wrapper;

public class Result : IResult
{
    public bool Succeeded { get; set; }
    public List<string> Messages { get; set; } = new List<string>();

    protected Result(bool isSuccess = true)
    {
        Succeeded = isSuccess;
    }
    protected Result(bool isSuccess = true, params string[] messages)
    {
        Succeeded = isSuccess;
        Messages = new List<string>(messages);
    }

    public static implicit operator Result(bool isSuccess) => new(isSuccess);
    public static implicit operator bool(Result result) => result.Succeeded;
    
    public static IResult Fail() 
        => new Result(false);
    public static IResult Fail(params string[] messages) 
        => new Result(false, messages);
    public static Task<IResult> FailAsync()
        => Task.FromResult(Fail());
    public static Task<IResult> FailAsync(params string[] messages)
        => Task.FromResult(Fail(messages));

    public static IResult Success()
        => new Result();
    public static IResult Success(string message)
        => new Result(true, message);
    public static Task<IResult> SuccessAsync()
        => Task.FromResult(Success());
    public static Task<IResult> SuccessAsync(string message)
        => Task.FromResult(Success(message));
}

public class Result<T> : Result, IResult<T>
{
    public T? Data { get; }
    
    private Result(T value) : base(true)
    {
        Data = value;
    }
    private Result(T value, params string[] messages) : base(true, messages)
    {
        Data = value;
    }
    private Result(bool isSucceed = true) : base(isSucceed){}
    private Result(bool isSucceed = true, params string[] messages) : base(isSucceed, messages){}

    public TR Match<TR>(Func<T, TR> succ, Func<List<string>, TR> fail) =>
        Succeeded && Data is not null
            ? succ(Data)
            : fail(Messages);
    
    public static implicit operator Result<T>(T value) => new(value);
    
    public new static Result<T> Fail() 
        => new(false);
    public new static Result<T> Fail(params string[] messages) 
        => new(false, messages);
    public new static Task<Result<T>> FailAsync()
        => Task.FromResult(Fail());
    public new static Task<Result<T>> FailAsync(params string[] messages)
        => Task.FromResult(Fail(messages));

    public new static Result<T> Success()
        => new(true);
    public new static Result<T> Success(params string[] messages)
        => new(true, messages);
    public static Result<T> Success(T data)
        => new(data);
    public static Result<T> Success(T data, params string[] messages)
        => new(data, messages);
    public new static Task<Result<T>> SuccessAsync()
        => Task.FromResult(Success());
    public new static Task<Result<T>> SuccessAsync(params string[] messages)
        => Task.FromResult(Success(messages));
    public static Task<Result<T>> SuccessAsync(T data)
        => Task.FromResult(Success(data));
    public static Task<Result<T>> SuccessAsync(T data, params string[] messages)
        => Task.FromResult(Success(data, messages));
}