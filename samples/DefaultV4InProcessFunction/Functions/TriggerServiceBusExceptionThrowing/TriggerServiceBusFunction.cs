using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DefaultV4InProcessFunction.Functions.TriggerServiceBusExceptionThrowing
{
    public class TriggerServiceBusExceptionThrowingFunction
    {
        [FunctionName("TriggerServiceBusExceptionThrowingFunction")]
        public static IActionResult RunGetTriggerServiceBusException(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus-exception")]
            HttpRequest request,
            [ServiceBus("defaultv4inprocess-exception-queue", Connection = "ServiceBusConnection")]
            out string message)
        {
            message = "{ 'Name': 'SomeName' }";

            return new OkResult();
        }
    }
}