using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure.Telemetry;

public class CustomHttpDependencyFilter : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

    public CustomHttpDependencyFilter(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is DependencyTelemetry dependency &&
            "CustomHTTP".Equals(dependency.Type, StringComparison.Ordinal))
        {
            return;
        }

        _next.Process(item);
    }
}