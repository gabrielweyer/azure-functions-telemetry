using System.Text;
using Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.Telemetry;

#pragma warning disable CA1001 // The container is responsible for disposing the injected instances
public class TelemetryFunction
#pragma warning restore CA1001
{
    private readonly TestingChannel _testingChannel;
    private readonly TestingOptions _testingOptions;

    public TelemetryFunction(ITelemetryChannel testingChannel, IOptions<TestingOptions> testingOptions)
    {
        _testingOptions = testingOptions.Value;

        if (_testingOptions.IsEnabled)
        {
            _testingChannel = (TestingChannel)testingChannel;
        }
        else
        {
            _testingChannel = new TestingChannel();
        }
    }

    [FunctionName("GetTelemetry")]
    public IActionResult RunGetTelemetry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "telemetry")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request,
        ExecutionContext executionContext)
    {
        if (!_testingOptions.IsEnabled)
        {
            return GenerateTestingDisabledConflict(executionContext.InvocationId.ToString("D"));
        }

        var serialisedTelemetryItems = JsonSerializer.Serialize(_testingChannel.TelemetryItems, false);
        return new OkObjectResult(Encoding.UTF8.GetString(serialisedTelemetryItems));
    }

    [FunctionName("DeleteTelemetry")]
    public IActionResult RunDeleteTelemetry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "telemetry")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request,
        ExecutionContext executionContext)
    {
        if (!_testingOptions.IsEnabled)
        {
            return GenerateTestingDisabledConflict(executionContext.InvocationId.ToString("D"));
        }

        _testingChannel.Clear();
        return new AcceptedResult();
    }

    private static ObjectResult GenerateTestingDisabledConflict(string invocationId)
    {
        var problemDetails = new ProblemDetails
        {
            Detail = "This endpoint is only available when testing is enabled.",
            Instance = invocationId,
            Status = StatusCodes.Status409Conflict,
            Title = "TestingDisabled"
        };
        return new ObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" },
            StatusCode = problemDetails.Status
        };
    }
}
