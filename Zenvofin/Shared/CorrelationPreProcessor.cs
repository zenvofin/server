using FastEndpoints;
using Serilog.Context;

namespace Zenvofin.Shared;

public class CorrelationPreProcessor : IGlobalPreProcessor
{
    public async Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        using (LogContext.PushProperty("CorrelationId", context.HttpContext.TraceIdentifier))
        {
            await Task.CompletedTask;
        }
    }
}