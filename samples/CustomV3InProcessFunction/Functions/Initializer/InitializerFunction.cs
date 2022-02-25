using System.Linq;
using Gabo.AzureFunctionTelemetry.Samples.CustomV3InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV3InProcessFunction.Functions.Initializer
{
    public class InitializerFunction
    {
        private readonly TelemetryCounterInitializer _telemetryCounterInitializer;

        public InitializerFunction(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryCounterInitializer = telemetryConfiguration.TelemetryInitializers
                .Single(p => p is TelemetryCounterInitializer) as TelemetryCounterInitializer;
        }

        [FunctionName(nameof(InitializerFunction))]
        public IActionResult RunGetProcessor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "initializer")]
            HttpRequest request)
        {
            return new OkObjectResult(new
            {
                _telemetryCounterInitializer.AvailabilityTelemetryCount,
                _telemetryCounterInitializer.DependencyTelemetryCount,
                _telemetryCounterInitializer.EventTelemetryCount,
                _telemetryCounterInitializer.ExceptionTelemetryCount,
                _telemetryCounterInitializer.MetricTelemetryCount,
                _telemetryCounterInitializer.PageViewPerformanceTelemetryCount,
                _telemetryCounterInitializer.PageViewTelemetryCount,
                _telemetryCounterInitializer.RequestTelemetryCount,
                _telemetryCounterInitializer.TraceTelemetryCount,
                _telemetryCounterInitializer.OtherTelemetryCount
            });
        }
    }
}