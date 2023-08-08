using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.HttpExceptionThrowing;

public static class HttpExceptionThrowingFunction
{
    [Function(nameof(HttpExceptionThrowingFunction))]
    public static HttpResponseData RunGetException(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "http-exception")]
        HttpRequestData request)
    {
        throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
    }
}
