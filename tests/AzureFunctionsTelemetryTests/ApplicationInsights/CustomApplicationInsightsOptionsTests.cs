namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class CustomApplicationInsightsOptionsTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyOrWhiteSpaceHealthCheckFunctionName_ThenThrows(string functionName)
    {
        // Act & Assert
        Assert.ThrowsAny<Exception>(() =>
            new CustomApplicationInsightsOptions(
                "some-name",
                typeof(StringHelper),
                healthCheckFunctionName: functionName));
    }
}
