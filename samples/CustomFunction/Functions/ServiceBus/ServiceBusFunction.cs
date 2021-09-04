using Microsoft.Azure.WebJobs;

namespace CustomFunction.Functions.ServiceBus
{
    public class ServiceBusFunction
    {
        [FunctionName("ServiceBusFunction")]
        public static void Run(
            [ServiceBusTrigger("custom-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
        }
    }
}