using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class FunctionsFinderTests
{
    private readonly List<string> _foundFunctions;

    public FunctionsFinderTests()
    {
        _foundFunctions = FunctionsFinder.GetServiceBusTriggeredFunctionNames(typeof(FunctionsFinderTests));
    }

    [Fact]
    public void GivenStaticServiceBusFunction_ThenFound()
    {
        // Assert
        _foundFunctions.Should().Contain(nameof(StaticServiceBusFunction));
    }

    [Fact]
    public void GivenInstanceServiceBusFunction_ThenFound()
    {
        // Assert
        _foundFunctions.Should().Contain(nameof(InstanceServiceBusFunction));
    }

    [Fact]
    public void GivenHttpFunction_ThenNotFound()
    {
        // Assert
        _foundFunctions.Should().NotContain(nameof(HttpFunction));
    }
}

public static class StaticServiceBusFunction
{
    [FunctionName(nameof(StaticServiceBusFunction))]
    public static void Run(
        [ServiceBusTrigger("static-queue", Connection = "ServiceBusConnection")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        string myQueueItem)
    {
    }
}

public static class InstanceServiceBusFunction
{
    [FunctionName(nameof(InstanceServiceBusFunction))]
    public static void Run(
        [ServiceBusTrigger("instance-queue", Connection = "ServiceBusConnection")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        string myQueueItem)
    {
    }
}

public static class HttpFunction
{
    [HttpGet]
    [FunctionName(nameof(HttpFunction))]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "http")]
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request) =>
        new OkResult();
}
