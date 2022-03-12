using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.Health;

public static class HealthFunction
{
    [HttpGet]
    [FunctionName(nameof(HealthFunction))]
    public static IActionResult RunHeadHealth(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request) =>
        new OkResult();
}
