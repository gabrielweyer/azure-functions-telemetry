using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Functions.Health;

public static class HealthFunction
{
    [HttpGet]
    [FunctionName(nameof(HealthFunction))]
    public static IActionResult RunHeadHealth(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")]
        HttpRequest request) =>
        new OkResult();
}

