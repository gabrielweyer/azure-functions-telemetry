using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Functions.TraceLog;

public class TraceLogFunction
{
    private readonly ILogger<TraceLogFunction> _logger;

    public TraceLogFunction(ILogger<TraceLogFunction> logger)
    {
        _logger = logger;
    }

    [FunctionName(nameof(TraceLogFunction))]
    public IActionResult RunGetVerboseLog(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "trace-log")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        _logger.LogTrace("I'm a log with Personally Identifiable Information");

        return new AcceptedResult();
    }
}
