namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal abstract class TelemetryItem
{
    public string Name { get; set; } = default!;
    public TelemetryItemTags Tags { get; set; } = default!;
    public string OperationId => Tags.OperationId;
    public string OperationName => Tags.OperationName;
}

internal sealed class TelemetryItemTags
{
    [JsonProperty("ai.application.ver")]
    public string? ApplicationVersion { get; set; }

    [JsonProperty("ai.cloud.role")]
    public string? CloudRoleName { get; set; }

    [JsonProperty("ai.operation.id")]
    public string OperationId { get; set; } = default!;

    [JsonProperty("ai.operation.name")]
    public string OperationName { get; set; } = default!;
}
