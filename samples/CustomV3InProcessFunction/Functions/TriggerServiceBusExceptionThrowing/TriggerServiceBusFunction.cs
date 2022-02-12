using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CustomV3InProcessFunction.Functions.TriggerServiceBusExceptionThrowing
{
    public static class TriggerServiceBusExceptionThrowingFunction
    {
        [FunctionName(nameof(TriggerServiceBusExceptionThrowingFunction))]
        public static IActionResult RunGetTriggerServiceBusException(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-bus-exception")]
            HttpRequest request,
            [ServiceBus("customv3inprocess-exception-queue", Connection = "ServiceBusConnection")]
            out string message)
        {
            message = "{ 'Name': 'SomeName' }";

            return new OkResult();
        }
    }
}