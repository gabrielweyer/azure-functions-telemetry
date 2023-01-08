namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal sealed class DefaultTelemetryFunctionClient : TelemetryFunctionClient
{
    public DefaultTelemetryFunctionClient() : base(7073)
    {
    }
}
