using System.Net;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.Dependency;

public class DependencyFunction
{
    private readonly TelemetryClient _telemetryClient;

    public DependencyFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryClient = new TelemetryClient(telemetryConfiguration);
    }

    [Function(nameof(DependencyFunction))]
    public HttpResponseData RunGetAppInsightsDependency(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dependency")]
        HttpRequestData request)
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

        return request.CreateResponse(HttpStatusCode.Accepted);
    }
}
