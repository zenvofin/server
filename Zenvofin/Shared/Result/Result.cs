namespace Zenvofin.Shared.Result;

public class Result
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = [];

    public ResultCode ResultCode { get; set; } = ResultCode.Ok;

    public static Result Success(string message)
        => new() { IsSuccess = true, Message = message };

    public static Result Success(string message, ResultCode resultCode)
        => new() { IsSuccess = true, Message = message, ResultCode = resultCode };

    public static Result Fail(string message, ResultCode resultCode)
        => new() { IsSuccess = false, Errors = [message], ResultCode = resultCode };

    public static Result Fail(List<string> errors, ResultCode resultCode)
        => new() { IsSuccess = false, Errors = errors, ResultCode = resultCode };

    public static Result Fail(string message)
        => new() { IsSuccess = false, Errors = [message], ResultCode = ResultCode.BadRequest };

    public static Result Fail(List<string> errors)
        => new() { IsSuccess = false, Errors = errors, ResultCode = ResultCode.BadRequest };
}

public sealed class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string message, ResultCode resultCode)
        => new() { IsSuccess = true, Data = data, Message = message, ResultCode = resultCode };

    public static Result<T> Success(T data, string message)
        => new() { IsSuccess = true, Data = data, Message = message };

    public static Result<T> Success(T data)
        => new() { IsSuccess = true, Data = data };

    public static new Result<T> Fail(string message, ResultCode resultCode)
        => new() { IsSuccess = false, Errors = [message], ResultCode = resultCode };

    public static new Result<T> Fail(List<string> errors, ResultCode resultCode)
        => new() { IsSuccess = false, Errors = errors, ResultCode = resultCode };

    public static new Result<T> Fail(string message)
        => new() { IsSuccess = false, Errors = [message], ResultCode = ResultCode.BadRequest };

    public static new Result<T> Fail(List<string> errors)
        => new() { IsSuccess = false, Errors = errors, ResultCode = ResultCode.BadRequest };
}