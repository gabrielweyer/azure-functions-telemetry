using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.TriggerServiceBus;

public static class TriggerServiceBusFunction
{
    [Function(nameof(TriggerServiceBusFunction))]
    public static TriggerServiceBusFunctionOutput RunGetTriggerServiceBus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus")]
        HttpRequestData request)
    {
        return new TriggerServiceBusFunctionOutput
        {
            Message = "{ 'Name': 'SomeName' }",
            Response = request.CreateResponse(HttpStatusCode.OK)
        };
    }
}
