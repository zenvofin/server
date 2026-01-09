namespace Zenvofin.Shared;

public abstract class ResultBase
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = [];
}

public sealed class Result : ResultBase
{
    public static Result Success(string message)
        => new() { IsSuccess = true, Message = message };

    public static Result Fail(string message)
        => new() { IsSuccess = false, Errors = [message] };

    public static Result Fail(List<string> errors)
        => new() { IsSuccess = false, Errors = errors };
}

public sealed class Result<T> : ResultBase
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string message)
        => new() { IsSuccess = true, Data = data, Message = message };

    public static Result<T> Success(T data)
        => new() { IsSuccess = true, Data = data };

    public static Result<T> Fail(string message)
        => new() { IsSuccess = false, Errors = [message] };

    public static Result<T> Fail(List<string> errors)
        => new() { IsSuccess = false, Errors = errors };
}