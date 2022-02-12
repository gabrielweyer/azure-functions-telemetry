using Custom.FunctionsTelemetry.ApplicationInsights;
using Custom.FunctionsTelemetryTests.TestInfrastructure.Builders;
using Xunit;

namespace Custom.FunctionsTelemetryTests.ApplicationInsights;

public class ServiceBusRequestInitializerTests
{
    private readonly ServiceBusRequestInitializer _target;

    public ServiceBusRequestInitializerTests()
    {
        _target = new ServiceBusRequestInitializer();
    }

    [Theory]
    [InlineData(true, "200")]
    [InlineData(false, "500")]
    public void GivenServiceBusBinding_ThenSetResponseCodeBasedOnSuccess(bool success, string expectedStatusCode)
    {
        // Arrange
        var requestTelemetry = RequestTelemetryBuilder.AsServiceBus();
        requestTelemetry.Success = success;

        // Act
        _target.Initialize(requestTelemetry);

        // Assert
        Assert.Equal(expectedStatusCode, requestTelemetry.ResponseCode);
    }

    [Fact]
    public void GivenServiceBusBinding_ThenSetUrlBasedOnName()
    {
        // Arrange
        var requestTelemetry = RequestTelemetryBuilder.AsServiceBus();

        // Act
        _target.Initialize(requestTelemetry);

        // Assert
        Assert.Equal("/ServiceBusFunction", requestTelemetry.Url.ToString());
    }

    [Fact]
    public void GivenServiceBusBinding_WhenSuccessIsNotSet_ThenDoNotSetResponseCode()
    {
        // Arrange
        var requestTelemetry = RequestTelemetryBuilder.AsServiceBus();
        requestTelemetry.Success = null;

        // Act
        _target.Initialize(requestTelemetry);

        // Assert
        Assert.Equal("0", requestTelemetry.ResponseCode);
    }

    [Fact]
    public void GivenHttpBinding_ThenDoNotOverrideResponseCode()
    {
        // Arrange
        var requestTelemetry = RequestTelemetryBuilder.AsHttp();
        requestTelemetry.ResponseCode = "202";

        // Act
        _target.Initialize(requestTelemetry);

        // Assert
        Assert.Equal("202", requestTelemetry.ResponseCode);
    }
}