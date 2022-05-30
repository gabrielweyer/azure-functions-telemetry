using FluentAssertions;
using Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;
using Xunit;

namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

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

    [Fact]
    public async Task GivenRequest_ThenDiscardExecutionTraces()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.LogVerboseAsync();

        // Assert
        var telemetryItems = await _client.GetTelemetryAsync();
        var requestTelemetry = telemetryItems.Should()
            .ContainSingle(i => i.Name == "AppRequests" && i.Tags.OperationName == "TraceLogFunction").Subject;
        telemetryItems.Should().ContainSingle(i => i.Tags.OperationId == requestTelemetry.Tags.OperationId);
    }

    [Fact]
    public async Task GivenHttpBinding_WhenException_ThenNoDuplicateExceptionTelemetry()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.ThrowOnHttpBindingAsync();

        // Assert
        var telemetryItems = await _client.GetTelemetryAsync();
        var requestTelemetry = telemetryItems.Should()
            .ContainSingle(i => i.Name == "AppRequests" && i.Tags.OperationName == "HttpExceptionThrowingFunction").Subject;
        telemetryItems.Should().ContainSingle(i => i.Name == "AppExceptions" && i.Tags.OperationId == requestTelemetry.Tags.OperationId);
    }

    [Fact]
    public async Task GivenHealthCheck_ThenRequestIsDiscarded()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.CheckHealthAsync();
        await _client.LogVerboseAsync();

        // Assert
        var telemetryItems = await _client.GetTelemetryAsync();
        telemetryItems.Should().NotContain(i => i.Name == "AppRequests" && i.Tags.OperationName == "HealthFunction");
        telemetryItems.Should().Contain(i => i.Name == "AppRequests" && i.Tags.OperationName == "TraceLogFunction");
    }
}
