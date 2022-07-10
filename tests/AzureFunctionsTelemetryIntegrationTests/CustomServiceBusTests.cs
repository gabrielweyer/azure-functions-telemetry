using FluentAssertions;
using Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;
using Xunit;

namespace Gabo.AzureFunctionsTelemetryIntegrationTests;

[Collection("Custom")]
public class CustomServiceBusTests
{
    private static readonly CustomTelemetryFunctionClient _client;

    static CustomServiceBusTests()
    {
        _client = new CustomTelemetryFunctionClient();
    }

    [Fact]
    public async Task GivenServiceBusRequest_ThenUrlIsFunctionNameAnd200StatusCode()
    {
        // Arrange
        await _client.DeleteTelemetryAsync();

        // Act
        await _client.TriggerServiceBusAsync();

        // Assert
        var request = await _client.GetTelemetryItemAsync<RequestItem>(i => i.Tags.OperationName == "ServiceBusFunction");
        request.Data.BaseData.Url.Should().Be("/ServiceBusFunction");
        request.Data.BaseData.ResponseCode.Should().Be("200");
    }
}
