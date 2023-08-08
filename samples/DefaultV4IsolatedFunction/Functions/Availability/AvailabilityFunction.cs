using System.Net;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.Availability;

public class AvailabilityFunction
{
    private readonly TelemetryClient _telemetryClient;

    public AvailabilityFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryClient = new TelemetryClient(telemetryConfiguration);
    }

    [Function(nameof(AvailabilityFunction))]
    public HttpResponseData RunGetAppInsightsAvailability(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "availability")]
        HttpRequestData request)
    {
        var availability = new AvailabilityTelemetry(
            "WeAreAvailable",
            DateTimeOffset.UtcNow,
            TimeSpan.FromMilliseconds(125),
            "FromSomewhere",
            true);

        _telemetryClient.TrackAvailability(availability);

        return request.CreateResponse(HttpStatusCode.Accepted);
    }
}
