using Microsoft.Azure.WebJobs;

namespace DefaultV4InProcessFunction.Functions.ServiceBus
{
    public class ServiceBusFunction
    {
        [FunctionName("ServiceBusFunction")]
        public static void Run(
            [ServiceBusTrigger("defaultv4inprocess-queue", Connection = "ServiceBusConnection")]
            string myQueueItem)
        {
        }
    }
}