using Microsoft.Extensions.Configuration;

namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

/// <summary>
/// Used to configure the Application Insights integration. Expected to be populated through the
/// <see cref="IConfiguration"/> abstraction.
/// </summary>
public class CustomApplicationInsightsOptions
{
    /// <summary>
    /// <para>This will discard the Service Bus trigger trace telemetry.</para>
    /// <para>The default value is <c>false</c>.</para>
    /// <para>Recommended on high-traffic services.</para>
    /// </summary>
    public bool DiscardServiceBusTrigger { get; set; }
    /// <summary>
    /// <para>This will discard the 'Executing ...' and 'Executed ...' Function execution traces.</para>
    /// <para>The default value is <c>true</c>.</para>
    /// <para>Typically you'll only disable this when raising an Azure support ticket for a performance issue.</para>
    /// </summary>
    public bool DiscardFunctionExecutionTraces { get; set; } = true;
    /// <summary>
    /// <para>We will discard all '200' HTTP status code requests for the specified Function.</para>
    /// <para>The default value is <c>null</c> (no request will be discarded).</para>
    /// <para>We only support a single health check function per Function App, but you can create a telemetry processor
    /// to discard other requests.</para>
    /// </summary>
    public string? HealthCheckFunctionName { get; set; }
}
