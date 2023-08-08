using Microsoft.Azure.Functions.Worker;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.ServiceBusExceptionThrowing;

public static class ServiceBusExceptionThrowingFunction
{
    [Function(nameof(ServiceBusExceptionThrowingFunction))]
    public static void Run(
        [ServiceBusTrigger("defaultv4isolated-exception-queue", Connection = "ServiceBusConnection")]
        string myQueueItem)
    {
        throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
    }
}
