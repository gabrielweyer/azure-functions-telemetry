using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DefaultFunction.Functions.TriggerServiceBusExceptionThrowing
{
    public class TriggerServiceBusExceptionThrowingFunction
    {
        [FunctionName("TriggerServiceBusExceptionThrowingFunction")]
        public static IActionResult RunGetTriggerServiceBusException(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus-exception")]
            HttpRequest request,
            [ServiceBus("default-exception-queue", Connection = "ServiceBusConnection")]
            out string message)
        {
            message = "{ 'Name': 'SomeName' }";

            return new OkResult();
        }
    }
}