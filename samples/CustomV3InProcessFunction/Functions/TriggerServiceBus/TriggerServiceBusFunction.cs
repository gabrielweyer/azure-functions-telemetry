using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CustomV3InProcessFunction.Functions.TriggerServiceBus
{
    public static class TriggerServiceBusFunction
    {
        [FunctionName(nameof(TriggerServiceBusFunction))]
        public static IActionResult RunGetTriggerServiceBus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus")]
            HttpRequest request,
            [ServiceBus("customv3inprocess-queue", Connection = "ServiceBusConnection")]
            out string message)
        {
            message = "{ 'Name': 'SomeName' }";

            return new OkResult();
        }
    }
}