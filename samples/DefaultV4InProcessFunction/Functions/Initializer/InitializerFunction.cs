using Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.Initializer;

public class InitializerFunction
{
    private readonly TelemetryCounterInitializer _telemetryCounterInitializer;

    public InitializerFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryCounterInitializer = (TelemetryCounterInitializer)telemetryConfiguration.TelemetryInitializers
            .Single(p => p is TelemetryCounterInitializer);
    }

    [FunctionName(nameof(InitializerFunction))]
    public IActionResult RunGetInitializer(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "initializer")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        return new OkObjectResult(
            new
            {
                _telemetryCounterInitializer.AvailabilityTelemetryCount,
                _telemetryCounterInitializer.DependencyTelemetryCount,
                _telemetryCounterInitializer.EventTelemetryCount,
                _telemetryCounterInitializer.ExceptionTelemetryCount,
                _telemetryCounterInitializer.MetricTelemetryCount,
                _telemetryCounterInitializer.RequestTelemetryCount,
                _telemetryCounterInitializer.TraceTelemetryCount
            });
    }
}
