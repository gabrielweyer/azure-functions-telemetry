using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Functions.InstanceInitializer;

public class InstanceInitializerFunction
{
    private readonly TelemetryCounterInstanceInitializer _telemetryCounterInstanceInitializer;

    public InstanceInitializerFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryCounterInstanceInitializer = telemetryConfiguration.TelemetryInitializers
            .Single(p => p is TelemetryCounterInstanceInitializer) as TelemetryCounterInstanceInitializer;
    }

    [FunctionName(nameof(InstanceInitializerFunction))]
    public IActionResult RunGetProcessor(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "instance-initializer")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        return new OkObjectResult(new
        {
            _telemetryCounterInstanceInitializer.AvailabilityTelemetryCount,
            _telemetryCounterInstanceInitializer.DependencyTelemetryCount,
            _telemetryCounterInstanceInitializer.EventTelemetryCount,
            _telemetryCounterInstanceInitializer.ExceptionTelemetryCount,
            _telemetryCounterInstanceInitializer.MetricTelemetryCount,
            _telemetryCounterInstanceInitializer.PageViewPerformanceTelemetryCount,
            _telemetryCounterInstanceInitializer.PageViewTelemetryCount,
            _telemetryCounterInstanceInitializer.RequestTelemetryCount,
            _telemetryCounterInstanceInitializer.TraceTelemetryCount,
            _telemetryCounterInstanceInitializer.OtherTelemetryCount
        });
    }
}
