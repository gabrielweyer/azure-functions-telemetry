using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.Health;

public static class HealthFunction
{
    [Function(nameof(HealthFunction))]
    public static HttpResponseData RunHeadHealth(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")]
        HttpRequestData request) =>
        request.CreateResponse(HttpStatusCode.OK);
}
