using System.Collections.Generic;
using Custom.FunctionsTelemetry.ApplicationInsights;
using FluentAssertions;
using Xunit;

namespace Custom.FunctionsTelemetryTests.ApplicationInsights
{
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
                ServiceBusTriggeredFunctionNames = new List<string>()
            };
            actualOptions.Should().BeEquivalentTo(expectedOptions);
        }

        [Fact]
        public void GivenDependencyFilterIsConfigured_ThenSetSetDependencyFilter()
        {
            // Arrange
            var builder = new CustomApplicationInsightsOptionsBuilder("some-name", typeof(StringHelper))
                .WithDependencyFilter("SomeDependency");

            // Act
            var actualOptions = builder.Build();

            // Assert
            var expectedOptions = new CustomApplicationInsightsOptions
            {
                ApplicationName = "some-name",
                TypeFromEntryAssembly = typeof(StringHelper),
                ServiceBusTriggeredFunctionNames = new List<string>(),
                DependencyTypeToFilter = "SomeDependency"
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
                ServiceBusTriggeredFunctionNames = new List<string>(),
                HealthCheckFunctionName = "HealthFunction"
            };
            actualOptions.Should().BeEquivalentTo(expectedOptions);
        }

        [Fact]
        public void GivenServiceBusDuplicateExceptionsConfigured_ThenSetServiceBusFunctionName()
        {
            // Arrange
            var serviceBusFunctionNames = new List<string> { "NameOne", "NameTwo" };
            var builder = new CustomApplicationInsightsOptionsBuilder("some-name", typeof(StringHelper))
                .DiscardServiceBusDuplicateExceptions(serviceBusFunctionNames);

            // Act
            var actualOptions = builder.Build();

            // Assert
            var expectedOptions = new CustomApplicationInsightsOptions
            {
                ApplicationName = "some-name",
                TypeFromEntryAssembly = typeof(StringHelper),
                ServiceBusTriggeredFunctionNames = serviceBusFunctionNames,
            };
            actualOptions.Should().BeEquivalentTo(expectedOptions);
        }

        [Fact]
        public void GivenServiceBusRequestInitializerConfigured_ThenEnableInitializer()
        {
            // Arrange
            var builder = new CustomApplicationInsightsOptionsBuilder("some-name", typeof(StringHelper))
                .WithServiceBusRequestInitializer();

            // Act
            var actualOptions = builder.Build();

            // Assert
            var expectedOptions = new CustomApplicationInsightsOptions
            {
                ApplicationName = "some-name",
                TypeFromEntryAssembly = typeof(StringHelper),
                ServiceBusTriggeredFunctionNames = new List<string>(),
                HasServiceBusRequestInitializer = true
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
                ServiceBusTriggeredFunctionNames = new List<string>(),
                HasServiceBusTriggerFilter = true
            };
            actualOptions.Should().BeEquivalentTo(expectedOptions);
        }
    }
}