using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV3InProcessFunction.Functions.TriggerServiceBusExceptionThrowing;

public static class TriggerServiceBusExceptionThrowingFunction
{
    [FunctionName(nameof(TriggerServiceBusExceptionThrowingFunction))]
    public static IActionResult RunGetTriggerServiceBusException(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus-exception")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request,
        [ServiceBus("defaultv3inprocess-exception-queue", Connection = "ServiceBusConnection")]
        out string message)
    {
        message = "{ 'Name': 'SomeName' }";

        return new OkResult();
    }
}
