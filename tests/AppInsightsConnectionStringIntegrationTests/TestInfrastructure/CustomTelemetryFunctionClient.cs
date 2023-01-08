namespace Gabo.AzureFunctionsTelemetryAppInsightsConnectionStringIntegrationTests.TestInfrastructure;

internal sealed class CustomTelemetryFunctionClient : TelemetryFunctionClient
{
    public CustomTelemetryFunctionClient() : base(7074)
    {
    }
}
