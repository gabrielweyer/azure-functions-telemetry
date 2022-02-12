using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CustomV4InProcessFunction.Functions.Availability;

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