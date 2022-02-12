using Microsoft.Azure.WebJobs;

namespace CustomV3InProcessFunction.Functions.ServiceBus
{
    public static class ServiceBusFunction
    {
        [FunctionName(nameof(ServiceBusFunction))]
        public static void Run(
            [ServiceBusTrigger("customv3inprocess-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
        }
    }
}