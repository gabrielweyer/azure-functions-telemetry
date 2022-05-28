using System.Collections.Concurrent;
using Microsoft.ApplicationInsights.Channel;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure.Telemetry;

public class TestingChannel : ITelemetryChannel
{
    public bool? DeveloperMode { get; set; }
    public string? EndpointAddress { get; set; }

    private readonly ConcurrentQueue<ITelemetry> _telemetryItems = new();
    public ITelemetry[] TelemetryItems => _telemetryItems.ToArray();

    public void Send(ITelemetry item)
    {
        _telemetryItems.Enqueue(item);
    }

    public void Clear()
    {
        _telemetryItems.Clear();
    }

    public void Flush()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
