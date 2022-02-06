using Microsoft.Azure.WebJobs;

namespace CustomV3InProcessFunction.Functions.ServiceBus
{
    public class ServiceBusFunction
    {
        [FunctionName("ServiceBusFunction")]
        public static void Run(
            [ServiceBusTrigger("customv3inprocess-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
        }
    }
}