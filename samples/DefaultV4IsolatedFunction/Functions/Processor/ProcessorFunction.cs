using System.Net;
using Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.Processor;

public class ProcessorFunction
{
    private readonly TelemetryCounterProcessor _telemetryCounterProcessor;

    public ProcessorFunction(TelemetryClient telemetryClient)
    {
        var processors = telemetryClient.TelemetryConfiguration.TelemetrySinks.First()
            .TelemetryProcessors;
        _telemetryCounterProcessor = (TelemetryCounterProcessor)processors.Single(p => p is TelemetryCounterProcessor);
    }

    [Function(nameof(ProcessorFunction))]
    public async Task<HttpResponseData> RunGetProcessorAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processor")]
        HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(
            new
            {
                _telemetryCounterProcessor.AvailabilityTelemetryCount,
                _telemetryCounterProcessor.DependencyTelemetryCount,
                _telemetryCounterProcessor.EventTelemetryCount,
                _telemetryCounterProcessor.ExceptionTelemetryCount,
                _telemetryCounterProcessor.MetricTelemetryCount,
                _telemetryCounterProcessor.RequestTelemetryCount,
                _telemetryCounterProcessor.TraceTelemetryCount
            }, cancellationToken: request.FunctionContext.CancellationToken);
        return response;
    }
}
