using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV3InProcessFunction.Infrastructure.Telemetry;

/// <summary>
/// <para>Even though this telemetry initializer enriches the telemetry, the main goal is to demonstrate that you need
/// to be careful when registering an instance of a telemetry initializer. Despite what the documentation states at:
/// https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#adding-telemetryinitializers
/// </para>
///
/// You should not use:
///
/// <code>
/// // This does not work, the telemetry initializer will not be registered
/// .AddSingleton(new TelemetryCounterInstanceInitializer("NiceValue"))
/// </code>
///
/// Instead you should use:
///
/// <code>
/// .AddSingleton&lt;ITelemetryInitializer&gt;(new TelemetryCounterInstanceInitializer("NiceValue"))
/// </code>
/// </summary>
internal sealed class TelemetryCounterInstanceInitializer : ITelemetryInitializer
{
    private readonly string _customPropertyValue;
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
    private const string CustomPropertyName = "NiceProp";

    public TelemetryCounterInstanceInitializer(string customPropertyValue)
    {
        _customPropertyValue = customPropertyValue;
    }

    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is ISupportProperties itemProperties &&
            !itemProperties.Properties.ContainsKey(CustomPropertyName))
        {
            itemProperties.Properties[CustomPropertyName] = _customPropertyValue;
        }

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
    }
}
