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
    public void GivenServiceBusDuplicateExceptionsConfigured_ThenSetServiceBusFunctionName()
    {
        // Arrange
        var serviceBusFunctionNames = new List<string> { "NameOne", "NameTwo" };
#pragma warning disable CS0618 // Even though it's obsolete, we still need to support it!
        var builder = new CustomApplicationInsightsOptionsBuilder("some-name", typeof(StringHelper))
            .DiscardServiceBusDuplicateExceptions(serviceBusFunctionNames);
#pragma warning restore CS0618

        // Act
        var actualOptions = builder.Build();

        // Assert
        var expectedOptions = new CustomApplicationInsightsOptions(
            "some-name",
            typeof(StringHelper),
            serviceBusTriggeredFunctionNames: serviceBusFunctionNames);
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
