using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Infrastructure.Telemetry;

/// <summary>
/// <para>This is not really a telemetry initializer as it doesn't enrich the telemetry. The goal is to demonstrate that
/// every telemetry item type is going through the initializer and could potentially be enriched.</para>
/// <para>When running in Azure you might get different results on each request as you might be hitting different
/// instances and the state is kept in-memory.</para>
/// </summary>
#pragma warning disable CA1812 // This type is instantiated by the Inversion of Control container
internal sealed class TelemetryCounterInitializer : ITelemetryInitializer
{
    public long AvailabilityTelemetryCount;
    public long DependencyTelemetryCount;
    public long EventTelemetryCount;
    public long ExceptionTelemetryCount;
    public long MetricTelemetryCount;
    public long RequestTelemetryCount;
    public long TraceTelemetryCount;

    public void Initialize(ITelemetry telemetry)
    {
        switch (telemetry)
        {
            case AvailabilityTelemetry:
                Interlocked.Increment(ref AvailabilityTelemetryCount);
                break;
            case DependencyTelemetry:
                Interlocked.Increment(ref DependencyTelemetryCount);
                break;
            case EventTelemetry:
                Interlocked.Increment(ref EventTelemetryCount);
                break;
            case ExceptionTelemetry:
                Interlocked.Increment(ref ExceptionTelemetryCount);
                break;
            case MetricTelemetry:
                Interlocked.Increment(ref MetricTelemetryCount);
                break;
            case RequestTelemetry:
                Interlocked.Increment(ref RequestTelemetryCount);
                break;
            case TraceTelemetry:
                Interlocked.Increment(ref TraceTelemetryCount);
                break;
        }
    }
}
