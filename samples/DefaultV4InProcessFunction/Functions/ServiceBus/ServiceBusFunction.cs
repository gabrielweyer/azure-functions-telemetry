using Microsoft.Azure.WebJobs;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.ServiceBus;

public static class ServiceBusFunction
{
    [FunctionName(nameof(ServiceBusFunction))]
    public static void Run(
        [ServiceBusTrigger("defaultv4inprocess-queue", Connection = "ServiceBusConnection")]
        string myQueueItem)
    {
    }
}