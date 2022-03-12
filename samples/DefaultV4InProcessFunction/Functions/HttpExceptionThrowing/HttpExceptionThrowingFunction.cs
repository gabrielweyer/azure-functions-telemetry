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
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
    }
}
