namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal class RequestItem : TelemetryItem
{
    public RequestData Data { get; set; } = new();
}

internal class RequestData
{
    public RequestBaseData BaseData { get; set; } = new();
}

internal class RequestBaseData
{
    public string ResponseCode { get; set; } = "0";
    public string? Url { get; set; }
}
