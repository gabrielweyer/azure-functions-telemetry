using System.Diagnostics;
using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction;

namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Custom")]
public sealed class CustomTelemetryTests : IDisposable
{
    private readonly CustomTelemetryFunctionClient _client;

    public CustomTelemetryTests()
    {
        _client = new CustomTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenTelemetry_ThenExpectedCloudRoleNameAndApplicationVersion()
    {
        // Act
        var telemetries = await _client.GetTelemetryAsync();

        // Assert
        var actualTelemetry = telemetries.First();
        var expectedVersion = FileVersionInfo.GetVersionInfo(typeof(Startup).Assembly.Location).ProductVersion;
        var expectedTags = new TelemetryItemTags
        {
            ApplicationVersion = expectedVersion,
            CloudRoleName = "customv4inprocess"
        };
        actualTelemetry.Tags.Should().BeEquivalentTo(expectedTags, options => options
            .Including(t => t.ApplicationVersion)
            .Including(t => t.CloudRoleName));
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
