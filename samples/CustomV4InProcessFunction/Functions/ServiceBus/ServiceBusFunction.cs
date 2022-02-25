using Microsoft.Azure.WebJobs;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Functions.ServiceBus;

public static class ServiceBusFunction
{
    [FunctionName(nameof(ServiceBusFunction))]
    public static void Run(
        [ServiceBusTrigger("customv4inprocess-queue", Connection = "ServiceBusConnection")]
        string myQueueItem)
    {
    }
}