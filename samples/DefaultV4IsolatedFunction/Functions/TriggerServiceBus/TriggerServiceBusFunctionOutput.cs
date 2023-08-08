using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.TriggerServiceBus;

public class TriggerServiceBusFunctionOutput
{
    [ServiceBusOutput("defaultv4isolated-queue", Connection = "ServiceBusConnection")]
    public string? Message { get; set; }

    public HttpResponseData? Response { get; set; }
}
