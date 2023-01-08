namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal sealed class TraceItem : TelemetryItem
{
    public TraceData Data { get; set; } = new();
}

internal sealed class TraceData
{
    public TraceBaseData BaseData { get; set; } = new();
}

internal sealed class TraceBaseData
{
    public string Message { get; set; } = default!;
}
