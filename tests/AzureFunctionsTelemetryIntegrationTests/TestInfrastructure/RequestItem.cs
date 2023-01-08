namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal sealed class RequestItem : TelemetryItem
{
    public RequestData Data { get; set; } = new();
}

internal sealed class RequestData
{
    public RequestBaseData BaseData { get; set; } = new();
}

internal sealed class RequestBaseData
{
    public string ResponseCode { get; set; } = "0";
    public string? Url { get; set; }
}
