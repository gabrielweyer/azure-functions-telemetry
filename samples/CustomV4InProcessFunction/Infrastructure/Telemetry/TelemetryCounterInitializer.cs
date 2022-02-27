using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure.Telemetry;

/// <summary>
/// This is not really a telemetry initializer as it doesn't enrich the telemetry. The goal is to demonstrate that
/// every telemetry item type is going through the initializer and could be potentially be enriched.
/// </summary>
public class TelemetryCounterInitializer : ITelemetryInitializer
{
    public long AvailabilityTelemetryCount;
    public long DependencyTelemetryCount;
    public long EventTelemetryCount;
    public long ExceptionTelemetryCount;
    public long MetricTelemetryCount;
    public long PageViewPerformanceTelemetryCount;
    public long PageViewTelemetryCount;
    public long RequestTelemetryCount;
    public long TraceTelemetryCount;
    public long OtherTelemetryCount;

    public void Initialize(ITelemetry telemetry)
    {
        switch (telemetry)
        {
            case AvailabilityTelemetry _:
                Interlocked.Increment(ref AvailabilityTelemetryCount);
                break;
            case DependencyTelemetry _:
                Interlocked.Increment(ref DependencyTelemetryCount);
                break;
            case EventTelemetry _:
                Interlocked.Increment(ref EventTelemetryCount);
                break;
            case ExceptionTelemetry _:
                Interlocked.Increment(ref ExceptionTelemetryCount);
                break;
            case MetricTelemetry _:
                Interlocked.Increment(ref MetricTelemetryCount);
                break;
            case PageViewPerformanceTelemetry _:
                Interlocked.Increment(ref PageViewPerformanceTelemetryCount);
                break;
            case PageViewTelemetry _:
                Interlocked.Increment(ref PageViewTelemetryCount);
                break;
            case RequestTelemetry _:
                Interlocked.Increment(ref RequestTelemetryCount);
                break;
            case TraceTelemetry _:
                Interlocked.Increment(ref TraceTelemetryCount);
                break;
            default:
                Interlocked.Increment(ref OtherTelemetryCount);
                break;
        }
    }
}