using Microsoft.Azure.WebJobs;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Functions.ServiceBusExceptionThrowing;

public static class ServiceBusExceptionThrowingFunction
{
    [FunctionName(nameof(ServiceBusExceptionThrowingFunction))]
    public static void Run(
        [ServiceBusTrigger("customv4inprocess-exception-queue", Connection = "ServiceBusConnection")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        string myQueueItem)
    {
        throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
    }
}
