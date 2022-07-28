namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class CustomApplicationInsightsOptionsBuilderTests
{
    [Fact]
    public void GivenBasicConfiguration_ThenSetApplicationNameAndEntryType()
    {
        // Arrange
        var builder = new CustomApplicationInsightsOptionsBuilder("some-name", typeof(StringHelper));

        // Act
        var actualOptions = builder.Build();

        // Assert
        var expectedOptions = new CustomApplicationInsightsOptions("some-name", typeof(StringHelper));
        actualOptions.Should().BeEquivalentTo(expectedOptions);
    }

    [Fact]
    public void GivenHealthFilterIsConfigured_ThenSetHealthFilter()
    {
        // Arrange
        var builder = new CustomApplicationInsightsOptionsBuilder("some-name", typeof(StringHelper))
            .WithHealthRequestFilter("HealthFunction");

        // Act
        var actualOptions = builder.Build();

        // Assert
        var expectedOptions = new CustomApplicationInsightsOptions(
            "some-name",
            typeof(StringHelper),
            healthCheckFunctionName: "HealthFunction");
        actualOptions.Should().BeEquivalentTo(expectedOptions);
    }

    [Fact]
    public void GivenServiceBusRequestTriggerFilterConfigured_ThenEnableFilter()
    {
        // Arrange
        var builder = new CustomApplicationInsightsOptionsBuilder("some-name", typeof(StringHelper))
            .WithServiceBusTriggerFilter();

        // Act
        var actualOptions = builder.Build();

        // Assert
        var expectedOptions = new CustomApplicationInsightsOptions(
            "some-name",
            typeof(StringHelper),
            true);
        actualOptions.Should().BeEquivalentTo(expectedOptions);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyOrWhiteSpaceHealthCheckFunctionName_ThenThrows(string functionName)
    {
        // Arrange
        var builder = new CustomApplicationInsightsOptionsBuilder("some-name", typeof(StringHelper));

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => builder.WithHealthRequestFilter(functionName));
    }
}
