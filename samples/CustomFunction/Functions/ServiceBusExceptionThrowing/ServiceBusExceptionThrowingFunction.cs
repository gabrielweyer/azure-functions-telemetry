using System;
using Microsoft.Azure.WebJobs;

namespace CustomFunction.Functions.ServiceBusExceptionThrowing
{
    public static class ServiceBusExceptionThrowingFunction
    {
        [FunctionName("ServiceBusExceptionThrowingFunction")]
        public static void Run(
            [ServiceBusTrigger("custom-exception-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
            throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
        }
    }
}
