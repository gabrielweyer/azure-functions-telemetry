namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class CustomApplicationInsightsOptionsTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyOrWhiteSpaceConfigurationSectionName_ThenThrows(string functionName)
    {
        // Act & Assert
        Assert.ThrowsAny<Exception>(() =>
            new CustomApplicationInsightsConfig(
                "some-name",
                typeof(StringHelper),
                configurationSectionName: functionName));
    }
}
