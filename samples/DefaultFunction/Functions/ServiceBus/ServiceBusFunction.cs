using Microsoft.Azure.WebJobs;

namespace DefaultFunction.Functions.ServiceBus
{
    public class ServiceBusFunction
    {
        [FunctionName("ServiceBusFunction")]
        public static void Run(
            [ServiceBusTrigger("default-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
        }
    }
}