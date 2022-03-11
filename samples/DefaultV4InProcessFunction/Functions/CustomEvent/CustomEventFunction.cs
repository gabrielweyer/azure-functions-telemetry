using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.CustomEvent;

public class CustomEventFunction
{
    private readonly TelemetryClient _telemetryClient;

    public CustomEventFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryClient = new TelemetryClient(telemetryConfiguration);
    }

    [FunctionName(nameof(CustomEventFunction))]
    public IActionResult RunGetAppInsightsEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "event")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        var @event = new EventTelemetry
        {
            Name = "SomethingHappened",
            Properties =
            {
                { "ImportantEventProperty", "SomeValue" }
            }
        };

        _telemetryClient.TrackEvent(@event);

        return new AcceptedResult();
    }
}
