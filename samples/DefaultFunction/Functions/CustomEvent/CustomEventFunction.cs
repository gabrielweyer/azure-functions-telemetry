using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DefaultFunction.Functions.CustomEvent
{
    public class CustomEventFunction
    {
        private readonly TelemetryClient _telemetryClient;

        public CustomEventFunction(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [FunctionName("CustomEventFunction")]
        public IActionResult RunGetAppInsightsEvent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "event")]
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
}
