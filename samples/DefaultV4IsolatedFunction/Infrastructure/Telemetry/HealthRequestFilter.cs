using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Infrastructure.Telemetry;

#pragma warning disable CA1812 // This type is instantiated by the Inversion of Control container
internal sealed class HealthRequestFilter : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

    public HealthRequestFilter(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is RequestTelemetry { ResponseCode: "200" } request &&
            "HealthFunction".Equals(request.Name, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _next.Process(item);
    }
}
