using Microsoft.Azure.WebJobs;

namespace DefaultV3InProcessFunction.Functions.ServiceBus
{
    public static class ServiceBusFunction
    {
        [FunctionName(nameof(ServiceBusFunction))]
        public static void Run(
            [ServiceBusTrigger("defaultv3inprocess-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
        }
    }
}