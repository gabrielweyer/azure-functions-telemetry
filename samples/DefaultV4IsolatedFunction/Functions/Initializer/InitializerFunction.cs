using System.Net;
using Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.Initializer;

public class InitializerFunction
{
    private readonly TelemetryCounterInitializer _telemetryCounterInitializer;

    public InitializerFunction(TelemetryConfiguration telemetryConfiguration)
    {
        _telemetryCounterInitializer = (TelemetryCounterInitializer)telemetryConfiguration.TelemetryInitializers
            .Single(p => p is TelemetryCounterInitializer);
    }

    [Function(nameof(InitializerFunction))]
    public async Task<HttpResponseData> RunGetInitializerAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "initializer")]
        HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(
            new
            {
                _telemetryCounterInitializer.AvailabilityTelemetryCount,
                _telemetryCounterInitializer.DependencyTelemetryCount,
                _telemetryCounterInitializer.EventTelemetryCount,
                _telemetryCounterInitializer.ExceptionTelemetryCount,
                _telemetryCounterInitializer.MetricTelemetryCount,
                _telemetryCounterInitializer.RequestTelemetryCount,
                _telemetryCounterInitializer.TraceTelemetryCount
            }, cancellationToken: request.FunctionContext.CancellationToken);
        return response;
    }
}
