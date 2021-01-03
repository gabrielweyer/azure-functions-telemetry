using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CustomApi
{
    public class DependencyFunction
    {
        private readonly TelemetryClient _telemetryClient;

        public DependencyFunction(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [FunctionName("DependencyFunction")]
        public IActionResult RunGetAppInsightsDependency(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dependency")]
            HttpRequest request)
        {
            var dependency = new DependencyTelemetry(
                "HTTP",
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
}