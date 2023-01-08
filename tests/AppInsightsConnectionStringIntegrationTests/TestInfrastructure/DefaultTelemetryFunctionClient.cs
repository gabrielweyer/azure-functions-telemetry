namespace Gabo.AzureFunctionsTelemetryAppInsightsConnectionStringIntegrationTests.TestInfrastructure;

internal sealed class DefaultTelemetryFunctionClient : TelemetryFunctionClient
{
    public DefaultTelemetryFunctionClient() : base(7073)
    {
    }
}
