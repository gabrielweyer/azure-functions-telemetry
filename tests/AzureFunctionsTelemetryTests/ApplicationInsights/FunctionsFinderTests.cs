using Gabo.AzureFunctionsTelemetryTests.TestInfrastructure.Functions;

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
