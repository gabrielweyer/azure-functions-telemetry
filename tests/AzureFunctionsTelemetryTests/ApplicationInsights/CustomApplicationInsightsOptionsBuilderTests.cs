using FluentAssertions;
using Gabo.AzureFunctionsTelemetry.ApplicationInsights;
using Xunit;

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
        var expectedOptions = new CustomApplicationInsightsOptions
        {
            ApplicationName = "some-name",
            TypeFromEntryAssembly = typeof(StringHelper),
#pragma warning disable CS0618 // Even though it's obsolete, we still need to support it!
            ServiceBusTriggeredFunctionNames = new List<string>()
#pragma warning restore CS0618
        };
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
        var expectedOptions = new CustomApplicationInsightsOptions
        {
            ApplicationName = "some-name",
            TypeFromEntryAssembly = typeof(StringHelper),
#pragma warning disable CS0618 // Even though it's obsolete, we still need to support it!
            ServiceBusTriggeredFunctionNames = new List<string>(),
#pragma warning restore CS0618
            HealthCheckFunctionName = "HealthFunction"
        };
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
        var expectedOptions = new CustomApplicationInsightsOptions
        {
            ApplicationName = "some-name",
            TypeFromEntryAssembly = typeof(StringHelper),
#pragma warning disable CS0618 // Even though it's obsolete, we still need to support it!
            ServiceBusTriggeredFunctionNames = serviceBusFunctionNames,
#pragma warning restore CS0618
        };
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
        var expectedOptions = new CustomApplicationInsightsOptions
        {
            ApplicationName = "some-name",
            TypeFromEntryAssembly = typeof(StringHelper),
#pragma warning disable CS0618 // Even though it's obsolete, we still need to support it!
            ServiceBusTriggeredFunctionNames = new List<string>(),
#pragma warning restore CS0618
            HasServiceBusTriggerFilter = true
        };
        actualOptions.Should().BeEquivalentTo(expectedOptions);
    }
}