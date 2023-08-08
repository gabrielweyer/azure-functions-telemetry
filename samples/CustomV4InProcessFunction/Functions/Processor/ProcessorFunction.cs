using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Functions.Processor;

public class ProcessorFunction
{
    private readonly TelemetryCounterProcessor _telemetryCounterProcessor;

    public ProcessorFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryCounterProcessor = (TelemetryCounterProcessor)telemetryConfiguration.TelemetryProcessors
            .Single(p => p is TelemetryCounterProcessor);
    }

    [FunctionName(nameof(ProcessorFunction))]
    public IActionResult RunGetProcessor(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processor")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        return new OkObjectResult(new
        {
            _telemetryCounterProcessor.AvailabilityTelemetryCount,
            _telemetryCounterProcessor.DependencyTelemetryCount,
            _telemetryCounterProcessor.EventTelemetryCount,
            _telemetryCounterProcessor.ExceptionTelemetryCount,
            _telemetryCounterProcessor.MetricTelemetryCount,
            _telemetryCounterProcessor.RequestTelemetryCount,
            _telemetryCounterProcessor.TraceTelemetryCount
        });
    }
}
