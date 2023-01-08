namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal sealed class TraceItem : TelemetryItem
{
    public TraceData Data { get; set; } = new();
}
