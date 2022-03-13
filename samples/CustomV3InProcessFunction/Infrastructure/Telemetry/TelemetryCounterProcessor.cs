using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV3InProcessFunction.Infrastructure.Telemetry;

/// <summary>
/// This is not really a telemetry processor as it doesn't discard any telemetry. The goal is to demonstrate that
/// every telemetry item type is going through the processor and could be potentially be discarded.
/// </summary>
public class TelemetryCounterProcessor : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

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

    public TelemetryCounterProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        switch (item)
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
            case PageViewPerformanceTelemetry:
                Interlocked.Increment(ref PageViewPerformanceTelemetryCount);
                break;
            case PageViewTelemetry:
                Interlocked.Increment(ref PageViewTelemetryCount);
                break;
            case RequestTelemetry:
                Interlocked.Increment(ref RequestTelemetryCount);
                break;
            case TraceTelemetry:
                Interlocked.Increment(ref TraceTelemetryCount);
                break;
            default:
                Interlocked.Increment(ref OtherTelemetryCount);
                break;
        }

        _next.Process(item);
    }
}
