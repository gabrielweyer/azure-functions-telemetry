using System;
using Microsoft.Azure.WebJobs;

namespace DefaultV3InProcessFunction.Functions.ServiceBusExceptionThrowing
{
    public static class ServiceBusExceptionThrowingFunction
    {
        [FunctionName("ServiceBusExceptionThrowingFunction")]
        public static void Run(
            [ServiceBusTrigger("defaultv3inprocess-exception-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
            throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
        }
    }
}
