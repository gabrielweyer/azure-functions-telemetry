using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV3InProcessFunction.Functions.Dependency;

public class DependencyFunction
{
    private readonly TelemetryClient _telemetryClient;

    public DependencyFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryClient = new TelemetryClient(telemetryConfiguration);
    }

    [FunctionName(nameof(DependencyFunction))]
    public IActionResult RunGetAppInsightsDependency(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dependency")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        var dependency = new DependencyTelemetry(
            "CustomHTTP",
            "AnotherSystem",
            "VeryImportantDependency",
            "/whatever",
            DateTimeOffset.UtcNow,
            TimeSpan.FromMilliseconds(125),
            "200",
            true);

        _telemetryClient.TrackDependency(dependency);

        return new AcceptedResult();
    }
}
