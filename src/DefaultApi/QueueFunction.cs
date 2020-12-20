using System;
using Microsoft.Azure.WebJobs;

namespace DefaultApi
{
    public static class QueueFunction
    {
        [FunctionName("QueueFunction")]
        public static void Run(
            [ServiceBusTrigger("default-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
            throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
        }
    }
}
