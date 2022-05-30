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

    [Fact]
    public async Task GivenRequest_ThenTwoExecutionTracesAreEmitted()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.LogVerboseAsync();

        // Assert
        var telemetryItems = await _client.GetTelemetryAsync();
        var requestTelemetry = telemetryItems.Should()
            .ContainSingle(i => i.Name == "AppRequests" && i.Tags.OperationName == "TraceLogFunction").Subject;
        var executionTraces = telemetryItems
            .Where(i => i.Name == "AppTraces" && i.Tags.OperationId == requestTelemetry.Tags.OperationId)
            .ToList();
        executionTraces.Should().HaveCount(2);
    }

    [Fact]
    public async Task GivenHttpBinding_WhenException_ThenDuplicateExceptionTelemetry()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.ThrowOnHttpBindingAsync();

        // Assert
        var telemetryItems = await _client.GetTelemetryAsync();
        var requestTelemetry = telemetryItems.Should()
            .ContainSingle(i => i.Name == "AppRequests" && i.Tags.OperationName == "HttpExceptionThrowingFunction").Subject;
        var exceptions = telemetryItems
            .Where(i => i.Name == "AppExceptions" && i.Tags.OperationId == requestTelemetry.Tags.OperationId)
            .ToList();
        exceptions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GivenHealthCheck_ThenRequestIsNotDiscarded()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.CheckHealthAsync();
        await _client.LogVerboseAsync();

        // Assert
        var telemetryItems = await _client.GetTelemetryAsync();
        telemetryItems.Should().Contain(i => i.Name == "AppRequests" && i.Tags.OperationName == "HealthFunction");
        telemetryItems.Should().Contain(i => i.Name == "AppRequests" && i.Tags.OperationName == "TraceLogFunction");
    }
}
