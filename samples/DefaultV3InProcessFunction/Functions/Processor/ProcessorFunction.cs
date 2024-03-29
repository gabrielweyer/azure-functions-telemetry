using Gabo.AzureFunctionTelemetry.Samples.DefaultV3InProcessFunction.Infrastructure.Telemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV3InProcessFunction.Functions.Processor;

public static class ProcessorFunction
{
    [FunctionName(nameof(ProcessorFunction))]
    public static IActionResult RunGetProcessor(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processor")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        return new OkObjectResult(new { TelemetryCounterProcessor.InvocationCount });
    }
}
