using FluentAssertions;
using Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;
using Xunit;

namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Custom")]
public class CustomTelemetryTests
{
    private static readonly CustomTelemetryFunctionClient _client;

    static CustomTelemetryTests()
    {
        _client = new CustomTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenTelemetry_ThenExpectedCloudRoleNameAndApplicationVersion()
    {
        // Act
        var telemetryItems = await _client.GetTelemetryAsync();

        // Assert
        var telemetryItem = telemetryItems.First();
        var expectedTags = new TelemetryItemTags
        {
            ApplicationVersion = "3.0.0.0",
            CloudRoleName = "customv4inprocess"
        };
        telemetryItem.Tags.Should().BeEquivalentTo(expectedTags, options => options
            .Including(t => t.ApplicationVersion)
            .Including(t => t.CloudRoleName));
    }
}
