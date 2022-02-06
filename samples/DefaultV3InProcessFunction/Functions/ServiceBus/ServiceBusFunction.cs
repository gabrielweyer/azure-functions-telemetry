using Microsoft.Azure.WebJobs;

namespace DefaultV3InProcessFunction.Functions.ServiceBus
{
    public class ServiceBusFunction
    {
        [FunctionName("ServiceBusFunction")]
        public static void Run(
            [ServiceBusTrigger("defaultv3inprocess-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
        }
    }
}