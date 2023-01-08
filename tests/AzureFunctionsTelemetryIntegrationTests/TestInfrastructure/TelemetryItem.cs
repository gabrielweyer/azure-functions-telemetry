namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal abstract class TelemetryItem
{
    public string Name { get; set; } = default!;
    public TelemetryItemTags Tags { get; set; } = default!;
    public string OperationId => Tags.OperationId;
    public string OperationName => Tags.OperationName;
}
