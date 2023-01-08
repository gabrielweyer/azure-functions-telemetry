using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.WebJobs;

namespace Gabo.AzureFunctionsTelemetryTests.TestInfrastructure.Functions;

public static class InstanceServiceBusFunction
{
    [FunctionName(nameof(InstanceServiceBusFunction))]
    public static void Run(
        [ServiceBusTrigger("instance-queue", Connection = "ServiceBusConnection")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        string myQueueItem)
    {
    }
}
