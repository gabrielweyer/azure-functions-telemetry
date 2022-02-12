using Custom.FunctionsTelemetry.ApplicationInsights;
using Custom.FunctionsTelemetryTests.TestInfrastructure.Builders;
using Xunit;

namespace Custom.FunctionsTelemetryTests.ApplicationInsights;

public class ApplicationInitializerTests
{
    private readonly ApplicationInitializer _target;
    private readonly ApplicationDescriptor _applicationDescriptor;

    public ApplicationInitializerTests()
    {
        _applicationDescriptor = new ApplicationDescriptor("applicationname-api", "20201202.2");

        _target = new ApplicationInitializer(_applicationDescriptor);
    }

    [Fact]
    public void GivenRequestTelemetry_ThenStampApplicationNameAndVersion()
    {
        // Arrange
        var requestTelemetry = RequestTelemetryBuilder
            .AsHttp();

        // Act
        _target.Initialize(requestTelemetry);

        // Assert
        Assert.Equal(_applicationDescriptor.Name, requestTelemetry.Context.Cloud.RoleName);
        Assert.Equal(_applicationDescriptor.Version, requestTelemetry.Context.Component.Version);
    }

    [Fact]
    public void GivenExceptionTelemetry_ThenStampApplicationNameAndVersion()
    {
        // Arrange
        var requestTelemetry = new ExceptionTelemetryBuilder("Function.FunctionName")
            .Build();

        // Act
        _target.Initialize(requestTelemetry);

        // Assert
        Assert.Equal(_applicationDescriptor.Name, requestTelemetry.Context.Cloud.RoleName);
        Assert.Equal(_applicationDescriptor.Version, requestTelemetry.Context.Component.Version);
    }

    [Fact]
    public void GivenTraceTelemetry_ThenStampApplicationNameAndVersion()
    {
        // Arrange
        var requestTelemetry = new TraceTelemetryBuilder("Function.FunctionName")
            .Build();

        // Act
        _target.Initialize(requestTelemetry);

        // Assert
        Assert.Equal(_applicationDescriptor.Name, requestTelemetry.Context.Cloud.RoleName);
        Assert.Equal(_applicationDescriptor.Version, requestTelemetry.Context.Component.Version);
    }
}