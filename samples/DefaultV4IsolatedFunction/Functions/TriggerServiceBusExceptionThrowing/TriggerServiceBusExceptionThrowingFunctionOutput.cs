using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.TriggerServiceBusExceptionThrowing;

public class TriggerServiceBusExceptionThrowingFunctionOutput
{
    [ServiceBusOutput("defaultv4isolated-exception-queue", Connection = "ServiceBusConnection")]
    public string? Message { get; set; }

    public HttpResponseData? Response { get; set; }
}
