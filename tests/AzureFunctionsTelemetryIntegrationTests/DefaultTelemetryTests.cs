using FluentAssertions;
using Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;
using Xunit;

namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

public class DefaultTelemetryTests
{
    private static readonly DefaultTelemetryFunctionClient _client;

    static DefaultTelemetryTests()
    {
        _client = new DefaultTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenTelemetry_ThenBothCloudRoleNameAndApplicationVersionAreEmpty()
    {
        // Act
        var telemetryItems = await _client.GetTelemetryAsync();

        // Assert
        var telemetryItem = telemetryItems.First();
        var expectedTags = new TelemetryItemTags
        {
            ApplicationVersion = null,
            CloudRoleName = null
        };
        telemetryItem.Tags.Should().BeEquivalentTo(expectedTags, options => options
            .Including(t => t.ApplicationVersion)
            .Including(t => t.CloudRoleName));
    }
}
