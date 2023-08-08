using System.Net;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.CustomEvent;

public class CustomEventFunction
{
    private readonly TelemetryClient _telemetryClient;

    public CustomEventFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryClient = new TelemetryClient(telemetryConfiguration);
    }

    [Function(nameof(CustomEventFunction))]
    public HttpResponseData RunGetAppInsightsEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "event")]
        HttpRequestData request)
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

        return request.CreateResponse(HttpStatusCode.Accepted);
    }
}
