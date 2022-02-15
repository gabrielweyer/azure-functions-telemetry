using System.Linq;
using CustomV3InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CustomV3InProcessFunction.Functions.Processor
{
    public class ProcessorFunction
    {
        private readonly TelemetryCounterProcessor _telemetryCounterProcessor;

        public ProcessorFunction(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryCounterProcessor = telemetryConfiguration.TelemetryProcessors
                .Single(p => p is TelemetryCounterProcessor) as TelemetryCounterProcessor;
        }

        [FunctionName(nameof(ProcessorFunction))]
        public IActionResult RunGetProcessor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processor")]
            HttpRequest request)
        {
            return new OkObjectResult(new
            {
                _telemetryCounterProcessor.AvailabilityTelemetryCount,
                _telemetryCounterProcessor.DependencyTelemetryCount,
                _telemetryCounterProcessor.EventTelemetryCount,
                _telemetryCounterProcessor.ExceptionTelemetryCount,
                _telemetryCounterProcessor.MetricTelemetryCount,
                _telemetryCounterProcessor.PageViewPerformanceTelemetryCount,
                _telemetryCounterProcessor.PageViewTelemetryCount,
                _telemetryCounterProcessor.RequestTelemetryCount,
                _telemetryCounterProcessor.TraceTelemetryCount,
                _telemetryCounterProcessor.OtherTelemetryCount
            });
        }
    }
}
