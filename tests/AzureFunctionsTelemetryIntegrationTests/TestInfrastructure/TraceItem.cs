namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal class TraceItem : TelemetryItem
{
    public TraceData Data { get; set; } = new();
}

internal class TraceData
{
    public TraceBaseData BaseData { get; set; } = new();
}

internal class TraceBaseData
{
    public string Message { get; set; } = default!;
}
