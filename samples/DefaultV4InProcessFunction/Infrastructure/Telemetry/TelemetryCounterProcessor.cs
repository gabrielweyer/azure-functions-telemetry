using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Infrastructure.Telemetry;

public class TelemetryCounterProcessor : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;
    private static int _invocationCount;
    public static int InvocationCount => Interlocked.CompareExchange(ref _invocationCount, 0, 0);

    public TelemetryCounterProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        Interlocked.Increment(ref _invocationCount);
        _next.Process(item);
    }
}
