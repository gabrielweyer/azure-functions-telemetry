using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CustomFunction.Functions.TriggerServiceBus
{
    public class TriggerServiceBusFunction
    {
        [FunctionName("TriggerServiceBusFunction")]
        public static IActionResult RunGetTriggerServiceBus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus")]
            HttpRequest request,
            [ServiceBus("custom-queue", Connection = "ServiceBusConnection")]
            out string message)
        {
            message = "{ 'Name': 'SomeName' }";

            return new OkResult();
        }
    }
}