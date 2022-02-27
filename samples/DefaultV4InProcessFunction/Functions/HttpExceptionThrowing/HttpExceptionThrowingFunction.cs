using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.HttpExceptionThrowing;

public static class HttpExceptionThrowingFunction
{
    [FunctionName(nameof(HttpExceptionThrowingFunction))]
    public static IActionResult RunGetException(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "http-exception")]
        HttpRequest req)
    {
        throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
    }
}