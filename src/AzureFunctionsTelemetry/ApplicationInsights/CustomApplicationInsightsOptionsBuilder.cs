namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

/// <summary>
/// This has been replaced by <see cref="CustomApplicationInsightsConfigBuilder"/>.
/// </summary>
[Obsolete(
    "This has been replaced by CustomApplicationInsightsConfigBuilder. See migration guide: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/docs/Migrate-from-v1-to-v2.md.",
    true)]
public class CustomApplicationInsightsOptionsBuilder
{
    /// <summary>
    /// This has been replaced by <see cref="CustomApplicationInsightsConfigBuilder"/>.
    /// </summary>
    /// <param name="applicationName"></param>
    /// <param name="typeFromEntryAssembly"></param>
    [Obsolete(
        "This has been replaced by CustomApplicationInsightsConfigBuilder. See migration guide: https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/docs/Migrate-from-v1-to-v2.md.",
        true)]
    public CustomApplicationInsightsOptionsBuilder(string applicationName, Type typeFromEntryAssembly)
    {
    }
}
