using Microsoft.Azure.WebJobs;

namespace CustomV4InProcessFunction.Functions.ServiceBus;

public static class ServiceBusFunction
{
    [FunctionName("ServiceBusFunction")]
    public static void Run(
        [ServiceBusTrigger("customv4inprocess-queue", Connection = "ServiceBusConnection")]
        string myQueueItem)
    {
    }
}