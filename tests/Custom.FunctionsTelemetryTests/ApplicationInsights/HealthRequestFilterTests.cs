using Custom.FunctionsTelemetry.TestInfrastructure.Builders;
using Custom.FunctionsTelemetry.TestInfrastructure.Mocks;
using Xunit;

namespace Custom.FunctionsTelemetry.ApplicationInsights;

public class HealthRequestFilterTests
{
    private readonly HealthRequestFilter _target;
    private readonly MockTelemetryProcessor _innerProcessor;

    public HealthRequestFilterTests()
    {
        _innerProcessor = new MockTelemetryProcessor();
        _target = new HealthRequestFilter(_innerProcessor, "HealthFunction");
    }

    [Fact]
    public void GivenSuccessfulHealthRequest_ThenDiscardTelemetry()
    {
        // Arrange
        var requestTelemetry = RequestTelemetryBuilder.AsHttp();
        requestTelemetry.Name = "HealthFunction";
        requestTelemetry.ResponseCode = "200";

        // Act
        _target.Process(requestTelemetry);

        // Assert
        Assert.False(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenUnsuccessfulHealthRequest_ThenKeepTelemetry()
    {
        // Arrange
        var requestTelemetry = RequestTelemetryBuilder.AsHttp();
        requestTelemetry.Name = "HealthFunction";
        requestTelemetry.ResponseCode = "500";

        // Act
        _target.Process(requestTelemetry);

        // Assert
        Assert.True(_innerProcessor.WasProcessorCalled);
    }

    [Fact]
    public void GivenOtherSuccessfulRequest_ThenKeepTelemetry()
    {
        // Arrange
        var requestTelemetry = RequestTelemetryBuilder.AsHttp();
        requestTelemetry.Name = "OtherFunction";
        requestTelemetry.ResponseCode = "200";

        // Act
        _target.Process(requestTelemetry);

        // Assert
        Assert.True(_innerProcessor.WasProcessorCalled);
    }
}