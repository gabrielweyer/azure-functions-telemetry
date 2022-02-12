using System;
using Microsoft.Azure.WebJobs;

namespace CustomV3InProcessFunction.Functions.ServiceBusExceptionThrowing
{
    public static class ServiceBusExceptionThrowingFunction
    {
        [FunctionName(nameof(ServiceBusExceptionThrowingFunction))]
        public static void Run(
            [ServiceBusTrigger("customv3inprocess-exception-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
            throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
        }
    }
}
