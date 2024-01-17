namespace Shared.Wrapper;

public class Result : IResult
{
    public bool Succeeded { get; set; }
    public List<string> Messages { get; set; } = [];

    protected Result(bool isSuccess = true)
    {
        Succeeded = isSuccess;
    }
    protected Result(List<string> messages, bool isSuccess = true)
    {
        Succeeded = isSuccess;
        Messages = messages;
    }

    public static implicit operator Result(bool isSuccess) => new(isSuccess);
    public static implicit operator bool(Result result) => result.Succeeded;
    
    public static Result Fail() 
        => new Result(false);
    public static Result Fail(List<string> messages) 
        => new Result(messages, false);
    public static Result Fail(params string[] messages) 
        => new Result([..messages], false);
    public static Task<Result> FailAsync()
        => Task.FromResult(Fail());
    public static Task<Result> FailAsync(List<string> messages)
        => Task.FromResult(Fail(messages));
    public static Task<Result> FailAsync(params string[] messages)
        => Task.FromResult(Fail(messages));

    public static Result Success()
        => new Result();
    public static Result Success(List<string> messages)
        => new Result(messages, true);
    public static Result Success(params string[] messages)
        => new Result([..messages], true);
    public static Task<Result> SuccessAsync()
        => Task.FromResult(Success());
    public static Task<Result> SuccessAsync(List<string> messages)
        => Task.FromResult(Success(messages));
    public static Task<Result> SuccessAsync(string message)
        => Task.FromResult(Success(message));
}

public class Result<T> : Result, IResult<T>
{
    public T? Data { get; }
    
    private Result(T value) : base(true)
    {
        Data = value;
    }
    private Result(T value, List<string> messages) : base(messages, true)
    {
        Data = value;
    }
    private Result(bool isSucceed = true) : base(isSucceed){}
    private Result(List<string> messages, bool isSucceed = true) : base(messages, isSucceed){}

    public TR Match<TR>(Func<T, TR> success, Func<List<string>, TR> fail) =>
        Succeeded && Data is not null
            ? success(Data)
            : fail(Messages);
    
    public static implicit operator Result<T>(T value) => new(value);
    
    public new static Result<T> Fail() 
        => new(false);
    public new static Result<T> Fail(params string[] messages) 
        => new([..messages], false);
    public new static Result<T> Fail(List<string> messages) 
        => new(messages, false);
    public new static Task<Result<T>> FailAsync()
        => Task.FromResult(Fail());

    public new static Task<Result<T>> FailAsync(Result result)
        => FailAsync(result.Messages);
    public new static Task<Result<T>> FailAsync(List<string> messages)
        => Task.FromResult(Fail(messages));
    public new static Task<Result<T>> FailAsync(params string[] messages)
        => Task.FromResult(Fail(messages));

    public new static Result<T> Success()
        => new(true);
    public new static Result<T> Success(params string[] messages)
        => new([..messages], true);
    public new static Result<T> Success(List<string> messages)
        => new(messages, true);
    public static Result<T> Success(T data)
        => new(data);
    public static Result<T> Success(T data, params string[] messages)
        => new(data, [..messages]);
    public new static Task<Result<T>> SuccessAsync()
        => Task.FromResult(Success());
    public static Task<Result<T>> SuccessAsync(params string[] messages)
        => Task.FromResult(Success(messages));
    public static Task<Result<T>> SuccessAsync(T data)
        => Task.FromResult(Success(data));
    public static Task<Result<T>> SuccessAsync(T data, params string[] messages)
        => Task.FromResult(Success(data, messages));
}