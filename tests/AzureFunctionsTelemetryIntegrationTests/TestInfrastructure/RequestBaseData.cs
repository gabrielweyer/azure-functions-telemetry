namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal sealed class RequestBaseData
{
    public string ResponseCode { get; set; } = "0";
    public string? Url { get; set; }
}
