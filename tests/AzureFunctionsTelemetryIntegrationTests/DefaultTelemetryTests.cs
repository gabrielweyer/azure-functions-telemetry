namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Default")]
public sealed class DefaultTelemetryTests : IDisposable
{
    private readonly DefaultTelemetryFunctionClient _client;

    public DefaultTelemetryTests()
    {
        _client = new DefaultTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenTelemetry_ThenBothCloudRoleNameAndApplicationVersionAreEmpty()
    {
        // Act
        var telemetries = await _client.GetTelemetryAsync();

        // Assert
        var actualTelemetry = telemetries.First();
        var expectedTags = new TelemetryItemTags
        {
            ApplicationVersion = null,
            CloudRoleName = null
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
