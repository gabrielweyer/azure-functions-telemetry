using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.TriggerServiceBusExceptionThrowing;

public static class TriggerServiceBusExceptionThrowingFunction
{
    [Function(nameof(TriggerServiceBusExceptionThrowingFunction))]
    public static TriggerServiceBusExceptionThrowingFunctionOutput RunGetTriggerServiceBusException(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus-exception")]
        HttpRequestData request)
    {
        return new TriggerServiceBusExceptionThrowingFunctionOutput
        {
            Message = "{ 'Name': 'SomeName' }",
            Response = request.CreateResponse(HttpStatusCode.OK)
        };
    }
}
