using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV3InProcessFunction.Functions.TriggerServiceBus;

public static class TriggerServiceBusFunction
{
    [FunctionName(nameof(TriggerServiceBusFunction))]
    public static IActionResult RunGetTriggerServiceBus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request,
        [ServiceBus("defaultv3inprocess-queue", Connection = "ServiceBusConnection")]
        out string message)
    {
        message = "{ 'Name': 'SomeName' }";

        return new OkResult();
    }
}
