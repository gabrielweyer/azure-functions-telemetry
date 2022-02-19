using System.Threading;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace DefaultV4InProcessFunction.Infrastructure.Telemetry;

public class TelemetryCounterProcessor : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;
    public static int InvocationCount;

    public TelemetryCounterProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        Interlocked.Increment(ref InvocationCount);
        _next.Process(item);
    }
}