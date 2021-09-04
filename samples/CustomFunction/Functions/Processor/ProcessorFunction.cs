using System.Linq;
using Custom.FunctionsTelemetry.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CustomFunction.Functions.Processor
{
    public class ProcessorFunction
    {
        private readonly TelemetryCounter _telemetryCounter;

        public ProcessorFunction(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryCounter = telemetryConfiguration.TelemetryProcessors
                .Single(p => p is TelemetryCounter) as TelemetryCounter;
        }

        [FunctionName("ProcessorFunction")]
        public IActionResult RunGetProcessor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processor")]
            HttpRequest request)
        {
            return new OkObjectResult(new
            {
                _telemetryCounter.AvailabilityTelemetryCount,
                _telemetryCounter.DependencyTelemetryCount,
                _telemetryCounter.EventTelemetryCount,
                _telemetryCounter.ExceptionTelemetryCount,
                _telemetryCounter.MetricTelemetryCount,
                _telemetryCounter.PageViewPerformanceTelemetryCount,
                _telemetryCounter.PageViewTelemetryCount,
                _telemetryCounter.PerformanceCounterTelemetryCount,
                _telemetryCounter.RequestTelemetryCount,
                _telemetryCounter.SessionStateTelemetryCount,
                _telemetryCounter.TraceTelemetryCount,
                _telemetryCounter.OtherTelemetryCount
            });
        }
    }
}
