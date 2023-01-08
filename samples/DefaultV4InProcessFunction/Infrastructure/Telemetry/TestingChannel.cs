using System.Collections.Concurrent;
using Microsoft.ApplicationInsights.Channel;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Infrastructure.Telemetry;

public sealed class TestingChannel : ITelemetryChannel
{
    public bool? DeveloperMode { get; set; }
    public string? EndpointAddress { get; set; }

    private readonly ConcurrentQueue<ITelemetry> _telemetryItems = new();
    public ICollection<ITelemetry> TelemetryItems => _telemetryItems.ToArray();

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
        throw new NotSupportedException("We keep the telemetry in memory, so flushing is meaningless.");
    }

    public void Dispose()
    {
        _telemetryItems.Clear();
    }
}
