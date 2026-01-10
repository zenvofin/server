using FastEndpoints;

namespace Zenvofin.Shared.Result;

public static class SendResultExtensions
{
    public static Task SendAsync<TRequest, TResponse>(
        this ResponseSender<TRequest, TResponse> sender,
        Result result,
        CancellationToken ct)
        where TRequest : notnull
        where TResponse : Result
        => sender.ResponseAsync((TResponse)result, (int)result.ResultCode, ct);
}