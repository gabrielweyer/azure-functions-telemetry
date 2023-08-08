using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Worker;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.ServiceBus;

public static class ServiceBusFunction
{
    [Function(nameof(ServiceBusFunction))]
    public static void Run(
        [ServiceBusTrigger("defaultv4isolated-queue", Connection = "ServiceBusConnection")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        string myQueueItem)
    {
    }
}
