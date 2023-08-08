using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.TraceLog;

public class TraceLogFunction
{
    private readonly ILogger<TraceLogFunction> _logger;

    public TraceLogFunction(ILogger<TraceLogFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(TraceLogFunction))]
    public HttpResponseData RunGetTraceLog(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "trace-log")]
        HttpRequestData request)
    {
        _logger.LogTrace("I'm a log with Personally Identifiable Information");
        return request.CreateResponse(HttpStatusCode.Accepted);
    }
}
