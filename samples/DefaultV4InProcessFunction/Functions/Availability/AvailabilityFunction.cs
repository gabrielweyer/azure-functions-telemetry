using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.Availability;

public class AvailabilityFunction
{
    private readonly TelemetryClient _telemetryClient;

    public AvailabilityFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryClient = new TelemetryClient(telemetryConfiguration);
    }

    [FunctionName(nameof(AvailabilityFunction))]
    public IActionResult RunGetAppInsightsAvailability(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "availability")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        var availability = new AvailabilityTelemetry(
            "WeAreAvailable",
            DateTimeOffset.UtcNow,
            TimeSpan.FromMilliseconds(125),
            "FromSomewhere",
            true);

        _telemetryClient.TrackAvailability(availability);

        return new AcceptedResult();
    }
}
