namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal sealed class RequestItem : TelemetryItem
{
    public RequestData Data { get; set; } = new();
}
