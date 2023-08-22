namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class CustomApplicationInsightsConfigTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyOrWhiteSpaceConfigurationSectionName_ThenThrows(string configurationSectionName)
    {
        // Act & Assert
        Assert.ThrowsAny<Exception>(() =>
            new CustomApplicationInsightsConfig(
                "some-name",
                typeof(StringHelper),
                configurationSectionName));
    }
}
